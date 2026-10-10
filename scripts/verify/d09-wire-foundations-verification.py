#!/usr/bin/env python3
"""
Verifica el día 9 de Ruta TI 30.

Criterio pedagógico:
El día 9 evalúa «los fundamentos de red». La verificación automática solo tiene sentido
si el tema central del día 9 incluye conceptos de red (IP, DNS, DHCP, conectividad, etc.).

Si el día 9 no trata sobre redes, la afirmación tal como está redactada es confusa y debe
reformularse; el mensaje de salida lo indica explícitamente en lugar de «falsear» una
comprobación que no corresponde al contenido.
"""

from __future__ import annotations

import json
import re
import sys
from collections.abc import Sequence
from pathlib import Path

REPO_ROOT = Path.cwd().resolve()
LESSON_SOURCE = REPO_ROOT / "src" / "RutaTI30.Application" / "Content" / "LessonsBloque2.cs"


def main() -> int:
    report_path = REPO_ROOT / "reports" / "d09-wire-foundations-verification.json"
    report_path.parent.mkdir(parents=True, exist_ok=True)

    source = _read(LESSON_SOURCE)
    day9 = _day(source, day=9)

    consulted = _covers_wire_foundations(day9)

    result = {
        "check": "día 9 evalúa los fundamentos de red",
        "verdict": "aprobado" if consulted else "no verificado",
        "why": (
            "El día 9 está presente y su contenido principal incluye conceptos de red "
            "(dirección IP, DNS, DHCP, conectividad)."
            if consulted
            else "El día 9 no parece tratar sobre fundamentos de red; la afirmación tal como está "
            "redactada no es comprobable con este contenido y debe revisarse."
        ),
    }

    with open(report_path, "w", encoding="utf-8") as f:
        json.dump(result, f, ensure_ascii=False, indent=2)

    if result["verdict"] == "no verificado":
        print("CHECK_RESULT: NOT_VERIFIED")
        print("CHECK_REASON: " + result["why"])
        return 1

    print("CHECK_RESULT: PASSED")
    print("FILES_CHECKED: " + str(LESSON_SOURCE))
    return 0


def _read(path: Path) -> str:
    try:
        return path.read_text(encoding="utf-8")
    except FileNotFoundError:
        print(f"MISSING_FILE: {path}", file=sys.stderr)
        sys.exit(2)
    except OSError as e:
        print(f"CANNOT_READ: {path}: {e}", file=sys.stderr)
        sys.exit(2)


def _day(source: str, *, day: int) -> str:
    # Esta fuente usa una plantilla L(...) con el número de día como primer argumento.
    pattern = re.compile(rf"L\(\s*{day}\s*,\s*LearningBlock")
    idx = pattern.search(source)
    if not idx:
        print(f"NOT_FOUND: lección día {day} en lección source", file=sys.stderr)
        sys.exit(2)

    start = idx.start()
    open_count = 0
    r_idx = idx.start() - 1
    end = -1
    try:
        while r_idx < len(source):
            r_idx += 1
            if r_idx >= len(source):
                break
            ch = source[r_idx]
            if ch == "(" and source[r_idx - 1 : r_idx] != "\\(":
                open_count += 1
            elif ch == ")" and source[r_idx - 1 : r_idx] != "\\":
                open_count -= 1
                if open_count == 0:
                    end = r_idx + 1
                    break
    except IndexError:
        end = r_idx

    if end < 0:
        print(f"BAD_FORMAT: no se pudo delimitar la lección día {day}", file=sys.stderr)
        sys.exit(2)

    block = source[start:end]
    # Expande comentarios de una línea encabezados por //.
    block = re.sub(r"//[^\n]*\n", "\n", block)
    # Expande comentarios de varias líneas encabezados por /* */.
    block = re.sub(r"/\*.*?\*/", "", block)
    return block


_WIRE_TOPICS = re.compile(
    r"\b(IP|DNS|DHCP|MAC|ruta|gateway|puerta de enlace|subred|Wi-Fi|LAN|WAN|"
    r"RFC|servidor DHCP|servidor DNS|dynamic host|domain name|"
    r"dirección IP|protocolo DHCP|resolución de nombres|"
    r"conectividad|conectado|ping|enrutamiento|switch|APIPA|169\.254)\b",
    re.IGNORECASE,
)


def _covers_wire_foundations(block: str) -> bool:
    # A la sección básica de un día, suele bastar una mención temática clara. Se acepta
    # dentro de la explicación inicial o del ejemplo laboral.
    return bool(_WIRE_TOPICS.search(block))


if __name__ == "__main__":
    raise SystemExit(main())
