#!/usr/bin/env python3
"""Verificación estática de regresión para Ruta TI 30 (offline, sin dotnet).

Comprobaciones:
  1. XML bien formado: todos los .xaml, .csproj, .props y .sln no aplica.
  2. Balance léxico de todos los .cs (llaves, paréntesis y corchetes), ignorando
     cadenas, caracteres, comentarios y textos interpolados.
  3. Todo `{Binding X}` de las vistas resuelve a un miembro declarado en los
     ViewModels/registros de la app (unión de miembros; detecta typos y propiedades
     inexistentes).
  4. Sin llamadas de red (HttpClient, WebRequest, WebClient, Dns, Socket, Process.Start).
  5. SQL parametrizado: ninguna sentencia concatena variables (heurística sobre
     Execute/CommandText con "+" o $"...").

Salida: imprime resultados por verificación y sale con 1 si algo falla.
Uso: python3 scripts/verify/static-regression.py
"""
from __future__ import annotations

import re
import sys
import xml.dom.minidom
from pathlib import Path

REPO = Path.cwd().resolve()
SRC = REPO / "src"
FAILURES: list[str] = []


def fail(msg: str) -> None:
    FAILURES.append(msg)
    print("  FAIL:", msg)


# ------------------------------------------------------------------ utilidades
def strip_noise(src: str) -> str:
    """Comentarios eliminados; cadenas marcadas con \\x00...\\x01 (contenido conservado,
    con los delimitadores de cadena sustituidos para no romper el balance)."""
    out: list[str] = []
    i, n = 0, len(src)
    while i < n:
        c = src[i]
        nxt = src[i + 1] if i + 1 < n else ""
        if c == "/" and nxt == "/":
            while i < n and src[i] != "\n":
                i += 1
        elif c == "/" and nxt == "*":
            i += 2
            while i + 1 < n and not (src[i] == "*" and src[i + 1] == "/"):
                i += 1
            i += 2
        elif c == "$" and nxt == '"':
            i += 1  # tratada como cadena normal abajo
            continue
        elif c == "@" and nxt == '"':
            i += 2
            buf: list[str] = []
            while i < n:
                if src[i] == '"' and i + 1 < n and src[i + 1] == '"':
                    buf.append('"')
                    i += 2
                    continue
                if src[i] == '"':
                    i += 1
                    break
                buf.append(src[i])
                i += 1
            out.append("\x00" + "".join(buf) + "\x01")
        elif c == '"':
            i += 1
            buf = []
            while i < n:
                if src[i] == "\\" and i + 1 < n:
                    nxtc = src[i + 1]
                    buf.append({"n": "\n", "t": "\t", "r": "", "\\": "\\",
                                '"': '"', "'": "'"}.get(nxtc, ""))
                    i += 2
                    continue
                if src[i] == '"':
                    i += 1
                    break
                buf.append(src[i])
                i += 1
            out.append("\x00" + "".join(buf) + "\x01")
        elif c == "'":
            i += 1
            while i < n and src[i] != "'":
                i += 2 if src[i] == "\\" else 1
            if i < n:
                i += 1
            out.append("\x00'\x01")
        else:
            out.append(c)
            i += 1
    return "".join(out)


def check_xml() -> None:
    print("[1] XML bien formado")
    files = sorted(list(SRC.rglob("*.xaml")) + list(REPO.rglob("*.csproj"))
                   + list(REPO.rglob("*.props")))
    ok = 0
    for f in files:
        try:
            xml.dom.minidom.parse(str(f))
            ok += 1
        except Exception as e:  # noqa: BLE001
            fail(f"XML inválido: {f.relative_to(REPO)}: {e}")
    print(f"  {ok}/{len(files)} correctos")


def check_balance() -> None:
    print("[2] Balance léxico de .cs")
    files = sorted(SRC.rglob("*.cs")) + sorted((REPO / "tests").rglob("*.cs"))
    ok = 0
    for f in files:
        clean = strip_noise(f.read_text(encoding="utf-8"))
        for open_c, close_c in (("{", "}"), ("(", ")"), ("[", "]")):
            depth = 0
            i = 0
            bad = False
            while i < len(clean):
                ch = clean[i]
                if ch == "\x00":
                    j = clean.find("\x01", i)
                    i = (j + 1) if j >= 0 else len(clean)
                    continue
                if ch == open_c:
                    depth += 1
                elif ch == close_c:
                    depth -= 1
                    if depth < 0:
                        bad = True
                        break
                i += 1
            if bad or depth != 0:
                fail(f"Balance roto {open_c}{close_c} en {f.relative_to(REPO)} (profundidad final {depth})")
                break
        else:
            ok += 1
    print(f"  {ok}/{len(files)} correctos")


def collect_members() -> set[str]:
    """Miembros declarados en ViewModels y registros de la app."""
    members: set[str] = set()
    for f in (SRC / "RutaTI30.App").rglob("*.cs"):
        text = f.read_text(encoding="utf-8")
        clean = strip_noise(text)
        # propiedades públicas/privadas con get/set
        members.update(re.findall(r"\b(?:public|private|internal|protected)[\w<>\[\],\s?]*\s(\w+)\s*\{\s*get", clean))
        # auto-propiedades { get; init; } / { get; set; }
        members.update(re.findall(r"\b(\w+)\s*\{\s*get;", clean))
        # registros posicionales (parámetros del ctor)
        for m in re.finditer(r"record\s+\w+(?:<[^>]+>)?\s*\(([^)]*)\)", clean):
            params = m.group(1)
            for p in params.split(","):
                pm = re.search(r"(\w+)\s*(?:=|$)", p.strip())
                if pm:
                    members.add(pm.group(1))
        # campos/atributos simples
        members.update(re.findall(r"\b(?:public|private|internal)\s+(?:readonly\s+)?[\w<>\[\],\s?]+\s(\w+)\s*[;=]", clean))
    # Miembros de los items de colecciones declarados en ViewModels (registros) ya cubiertos.
    return members


def check_bindings(members: set[str]) -> None:
    print("[3] Resolución de {Binding} en XAML")
    total = 0
    missing: set[str] = set()
    for f in sorted(SRC.rglob("*.xaml")):
        text = f.read_text(encoding="utf-8")
        for m in re.finditer(r"\{Binding\s+([^,}]+)\}", text):
            path = m.group(1).strip()
            if path.startswith(("RelativeSource", "Path=DataContext")):
                continue
            if path.startswith("Path="):
                path = path[4:].strip()
            if not path or path == ".":
                continue
            root = path.split(".")[0].split("[")[0].strip()
            if not root or root.startswith("$"):
                continue
            total += 1
            if root not in members:
                missing.add(f"{f.name}: {root}")
    for item in sorted(missing):
        fail(f"Binding sin miembro declarado -> {item}")
    print(f"  {total} bindings revisados, {len(missing)} sin resolver")


def check_no_network() -> None:
    print("[4] Sin llamadas de red")
    patterns = [r"\bHttpClient\b", r"\bWebRequest\b", r"\bWebClient\b", r"\bDns\.",
                r"\bSocket\b", r"Process\.Start", r"\bSmtpClient\b"]
    hits = 0
    for f in sorted(SRC.rglob("*.cs")):
        clean = strip_noise(f.read_text(encoding="utf-8"))
        for p in patterns:
            if re.search(p, clean):
                fail(f"Posible llamada de red en {f.relative_to(REPO)}: {p}")
                hits += 1
    if hits == 0:
        print("  sin llamadas de red en src/")


def check_sql() -> None:
    print("[5] SQL parametrizado (heurística)")
    hits = 0
    for f in sorted((SRC / "RutaTI30.Infrastructure").rglob("*.cs")):
        text = f.read_text(encoding="utf-8")
        for i, line in enumerate(text.splitlines(), 1):
            if re.search(r'CommandText\s*=\s*\$?"[^"]*\+\s*\w', line) or \
               re.search(r'CommandText\s*=\s*\$"[^"]*\{', line):
                fail(f"SQL con interpolación/concatenación: {f.name}:{i}")
                hits += 1
    if hits == 0:
        print("  sin interpolación de SQL detectada")


def main() -> int:
    check_xml()
    check_balance()
    members = collect_members()
    print(f"  miembros de VM/registros indexados: {len(members)}")
    check_bindings(members)
    check_no_network()
    check_sql()
    print()
    if FAILURES:
        print(f"STATIC_REGRESSION: FAILED ({len(FAILURES)})")
        return 1
    print("STATIC_REGRESSION: PASSED")
    return 0


if __name__ == "__main__":
    sys.exit(main())
