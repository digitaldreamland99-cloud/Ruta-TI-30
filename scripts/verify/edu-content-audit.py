#!/usr/bin/env python3
"""Auditoría educativa + inventario de contenido de Ruta TI 30 (offline, sin dotnet).

Extrae del código fuente real (no de documentación) los recursos educativos:
  - Lecciones de los 30 días (objetivo, dificultad, duración, preguntas).
  - Preguntas de evaluación por día (opciones, índice correcto, tema).
  - Banco de preguntas de entrevista (campos obligatorios, áreas).
  - Escenarios del simulador de Help Desk y laboratorios.

Validaciones duras (exit != 0 si fallan):
  - 30 días únicos 1..30, ids de lección únicos.
  - Cada pregunta: id único, >= 2 opciones, 0 <= correcto < nº opciones, explicación y tema.
  - Entrevistas: ids únicos, campos obligatorios no vacíos.
  - Escenarios: ids únicos, >= 2 preguntas de diagnóstico, >= 3 pasos, solución/escalamiento y documentación.

Salidas:
  - reports/content-inventory.json (inventario estructurado)
  - reports/content-inventory.md   (tabla legible por categoría, nivel, dificultad y estado)

Uso: python3 scripts/verify/edu-content-audit.py
"""
from __future__ import annotations

import json
import re
import sys
from pathlib import Path

REPO = Path.cwd().resolve()
CONTENT = REPO / "src" / "RutaTI30.Application" / "Content"
REPORTS = REPO / "reports"

FAILURES: list[str] = []
def fail(msg: str) -> None:
    FAILURES.append(msg)


# ---------------------------------------------------------------- lexer utils
def strip_noise(src: str) -> str:
    """Elimina comentarios y deja cadenas marcadas con \x00...\x01 para no confundir
    llaves/paréntesis dentro de texto con estructura real."""
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
                    buf.append({"n": "\n", "t": "\t", "r": "", "\\": "\\", '"': '"',
                                "'": "'"}.get(nxtc, ""))
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
            i += 1
            if i < n:
                i += 1
            out.append("\x00'\x01")
        else:
            out.append(c)
            i += 1
    return "".join(out)


def spans(src: str, opener: str, closer: str) -> list[tuple[int, int]]:
    """Devuelve (inicio, fin) de los grupos balanceados, ignorando el interior de
    cadenas (marcado con \x00…\x01)."""
    res: list[tuple[int, int]] = []
    depth = 0
    start = -1
    i = 0
    n = len(src)
    while i < n:
        ch = src[i]
        if ch == "\x00":
            j = src.find("\x01", i)
            i = (j + 1) if j >= 0 else n
            continue
        if ch == opener:
            if depth == 0:
                start = i
            depth += 1
        elif ch == closer and depth > 0:
            depth -= 1
            if depth == 0 and start >= 0:
                res.append((start, i))
                start = -1
        i += 1
    return res


def strings_between(block: str) -> list[str]:
    """Extrae las cadenas literales dentro de un bloque (ya sin ruido)."""
    return [block[m.start() + 1:m.end() - 1] for m in re.finditer(r"\x00(?:[^\x01])*\x01", block, re.S)]


def unescape(s: str) -> str:
    return (s.replace('\\"', '"').replace("\\n", " ").replace("\\\\", "\\")
             .replace("\\'", "'"))


def word_count(text: str) -> int:
    return len(re.findall(r"[A-Za-zÁÉÍÓÚÜÑáéíóúüñ0-9]+", text))


def read(name: str) -> str:
    p = CONTENT / name
    if not p.exists():
        fail(f"archivo ausente: {name}")
        return ""
    return p.read_text(encoding="utf-8")


# ------------------------------------------------------------------ lecciones
def audit_lessons() -> list[dict]:
    items: list[dict] = []
    days_seen: dict[int, str] = {}
    lesson_ids: dict[str, int] = {}
    q_ids_global: dict[str, int] = {}

    for bloque in range(1, 6):
        src = read(f"LessonsBloque{bloque}.cs")
        clean = strip_noise(src)
        for start, end in spans(clean, "(", ")"):
            # localizar el prefijo L( a este nivel: comprobamos el texto justo antes
            pre = clean[max(0, start - 6):start]
            if not re.search(r"\bL\s*$", pre):
                continue
            block = clean[start + 1:end]
            if not re.match(r"\s*\d+\s*,", block):
                continue  # no es una llamada L(día, …)
            head = strings_between(block)
            if not head:
                continue
            # primer argumento numérico: el día
            m = re.match(r"\s*(\d+)\s*,", block)
            if not m:
                continue
            day = int(m.group(1))
            lid = f"leccion-{day:02d}"
            if day in days_seen:
                fail(f"día duplicado: {day} ({lid} también en {days_seen[day]})")
            days_seen[day] = lid
            lesson_ids[lid] = lesson_ids.get(lid, 0) + 1

            difficulty = re.search(r",\s*(\d+)\s*,\s*\d+\s*,\s*new\[\]", block)
            minutes = re.search(r",\s*\d+\s*,\s*(\d+)\s*,\s*new\[\]", block)

            # preguntas Q(id, enunciado, opciones, correcto, explicación, tema)
            questions = []
            for qs, qe in spans(block, "(", ")"):
                preq = block[max(0, qs - 6):qs]
                if not re.search(r"\bQ\s*$", preq):
                    continue
                qb = block[qs + 1:qe]
                if not qb.lstrip().startswith("\x00"):
                    continue  # firma del helper Q, no una llamada
                strs = strings_between(qb)
                cm = re.search(r"\}\s*,\s*(\d+)\s*,", qb)
                if len(strs) < 3 or cm is None:
                    fail(f"{lid}: pregunta ilegible -> {qb[:80]!r}")
                    continue
                qid, statement, explanation, topic = strs[0], strs[1], strs[-2], strs[-1]
                correct = int(cm.group(1))
                # opciones = cadenas entre el primer { y su cierre
                opt_span = spans(qb, "{", "}")
                n_opts = len(strings_between(qb[opt_span[0][0]:opt_span[0][1]])) if opt_span else 0
                if n_opts < 2:
                    fail(f"{qid}: menos de 2 opciones ({n_opts})")
                if not (0 <= correct < n_opts):
                    fail(f"{qid}: índice correcto {correct} fuera de rango 0..{n_opts - 1}")
                if not explanation.strip():
                    fail(f"{qid}: sin explicación")
                if not topic.strip():
                    fail(f"{qid}: sin tema")
                q_ids_global[qid] = q_ids_global.get(qid, 0) + 1
                questions.append({"id": qid, "topic": topic, "options": n_opts, "correct": correct})

            texts = " ".join(head)
            items.append({
                "id": lid,
                "day": day,
                "block": bloque,
                "title": head[1] if len(head) > 1 else "",
                "objective": head[2] if len(head) > 2 else "",
                "difficulty": int(difficulty.group(1)) if difficulty else None,
                "minutes": int(minutes.group(1)) if minutes else None,
                "words": word_count(unescape(texts)),
                "questions": questions,
            })

    if sorted(days_seen) != list(range(1, 31)):
        fail(f"días presentes {sorted(days_seen)} != 1..30")
    for lid, n in lesson_ids.items():
        if n > 1:
            fail(f"id de lección duplicado: {lid}")
    for qid, n in q_ids_global.items():
        if n > 1:
            fail(f"id de pregunta duplicado: {qid}")
    return items


# --------------------------------------------------------------- entrevistas
def audit_interviews() -> list[dict]:
    src = read("InterviewQuestions.cs")
    clean = strip_noise(src)
    items = []
    ids: dict[str, int] = {}
    for s, e in spans(clean, "(", ")"):
        pre = clean[max(0, s - 6):s]
        if not re.search(r"\bP\s*$", pre):
            continue
        block = clean[s + 1:e]
        if not block.lstrip().startswith("\x00"):
            continue  # firma del método helper, no una llamada
        strs = strings_between(block)
        if len(strs) < 8:
            fail(f"entrevista ilegible ({len(strs)} cadenas): {block[:60]!r}")
            continue
        # Orden real: P(id, kind(no-cadena), area, enunciado, respuesta, explicación, …)
        qid, area, statement = strs[0], strs[1], strs[2]
        model, explanation = strs[3], strs[4]
        ids[qid] = ids.get(qid, 0) + 1
        if not statement.strip():
            fail(f"{qid}: sin enunciado")
        if not model.strip():
            fail(f"{qid}: sin respuesta modelo")
        if not explanation.strip():
            fail(f"{qid}: sin explicación")
        kind = re.search(r"InterviewKind\.(\w+)", block)
        # arrays tras la explicación
        arrays = [strings_between(block[a:b]) for a, b in spans(block, "{", "}")]
        arrays = [a for a in arrays if a]
        if len(arrays) < 3:
            fail(f"{qid}: faltan conceptos/errores/criterios")
        items.append({
            "id": qid,
            "kind": kind.group(1) if kind else "?",
            "area": area,
            "statement": unescape(statement),
            "statement_words": word_count(unescape(statement)),
            "model_words": word_count(unescape(model)),
            "concepts": arrays[0] if arrays else [],
            "mistakes": arrays[1] if len(arrays) > 1 else [],
            "criteria": arrays[2] if len(arrays) > 2 else [],
            "followups": arrays[3] if len(arrays) > 3 else [],
        })
    for qid, n in ids.items():
        if n > 1:
            fail(f"id de entrevista duplicado: {qid}")
    return items


# --------------------------------------------------------------- escenarios
def audit_scenarios() -> list[dict]:
    src = read("TicketPracticeScenarios.cs")
    clean = strip_noise(src)
    items = []
    ids: dict[str, int] = {}
    for s, e in spans(clean, "(", ")"):
        pre = clean[max(0, s - 6):s]
        if not re.search(r"\bnew\s*$", pre):
            continue
        block = clean[s + 1:e]
        strs = strings_between(block)
        if len(strs) < 4 or not strs[0].startswith("prac-"):
            continue
        sid = strs[0]
        ids[sid] = ids.get(sid, 0) + 1
        arrays = [strings_between(block[a:b]) for a, b in spans(block, "{", "}")]
        arrays = [a for a in arrays if a]
        diag = arrays[0] if arrays else []
        steps = arrays[1] if len(arrays) > 1 else []
        # resto de cadenas: solución y documentación (últimas dos tras las matrices)
        tail = strs[4:]
        solution = tail[-2] if len(tail) >= 2 else ""
        docs = tail[-1] if tail else ""
        if len(diag) < 2:
            fail(f"{sid}: menos de 2 preguntas de diagnóstico")
        if len(steps) < 3:
            fail(f"{sid}: menos de 3 pasos sugeridos")
        if not solution.strip():
            fail(f"{sid}: sin solución/escalamiento")
        if not docs.strip():
            fail(f"{sid}: sin documentación")
        items.append({
            "id": sid,
            "title": unescape(strs[1]),
            "category": re.search(r"TicketCategory\.(\w+)", block).group(1)
            if re.search(r"TicketCategory\.(\w+)", block) else "?",
            "impact": (re.search(r"ImpactLevel\.(\w+)", block).group(1)
                       if re.search(r"ImpactLevel\.(\w+)", block) else "?"),
            "urgency": (re.search(r"UrgencyLevel\.(\w+)", block).group(1)
                        if re.search(r"UrgencyLevel\.(\w+)", block) else "?"),
            "diagnostic_questions": len(diag),
            "steps": len(steps),
            "has_solution": bool(solution.strip()),
            "has_documentation": bool(docs.strip()),
        })
    for sid, n in ids.items():
        if n > 1:
            fail(f"id de escenario duplicado: {sid}")
    return items


# ------------------------------------------------------------ laboratorios
def audit_glossary() -> dict:
    src = read("Glossary.cs")
    clean = strip_noise(src)
    items: list[list[str]] = []
    ids: dict[str, int] = {}
    terms: dict[str, str] = {}   # id -> término (minúsculas) para detectar duplicados semánticos
    definitions: dict[str, str] = {}
    categories: set[str] = set()
    category_entries: dict[str, int] = {}
    for s, e in spans(clean, "(", ")"):
        pre = clean[max(0, s - 3):s]
        if not re.search(r"\bG\s*$", pre):
            continue
        block = clean[s + 1:e]
        strs = strings_between(block)
        if len(strs) < 7:
            continue
        # Firma: G(id, término, categoría, definición, técnica, ejemplo, error, relacionados, días)
        gid, term = strs[0], strs[1]
        if not gid or not term or "\n" in gid:
            continue
        category = strs[2] if len(strs) > 2 else ""
        categories.add(category)
        category_entries[category] = category_entries.get(category, 0) + 1
        if not category.strip():
            fail(f"{gid}: categoría vacía")
        ids[gid] = ids.get(gid, 0) + 1
        key = term.strip().lower()
        if key in terms:
            fail(f"glosario duplicado: «{term}» ya existe como id {terms[key]}")
        terms[key] = gid
        definitions[gid] = (strs[3].strip().lower() if len(strs) > 3 else "")
        # cadenas puras con '-' no vacías en el bloque tras el error = ids relacionados (sin días)
        related = [r for r in strs[7:] if re.fullmatch(r"[\w\-]+", r) and not re.fullmatch(r"\d+(,\s*\d+)*", r)]
        dip = [t for t in related if ids.get(t) is not None]
        if len(related) < 1:
            fail(f"{gid}: sin términos relacionados")
        for r in related:
            if r not in {i for i in ids}:
                pass  # validado al final contra id_set
        items.append([gid] + related)
    for gid, n in ids.items():
        if n > 1:
            fail(f"id de glosario duplicado: {gid} ({n} veces)")
    id_set = set(ids)
    dangling = sorted({r for e in items for r in e[1:] if r not in id_set})
    if dangling:
        fail(f"términos relacionados inexistentes en el glosario: {', '.join(dangling)}")

    # Duplicados casi idénticos: misma definición inicial (primera frase) en dos ids distintos.
    first_sentence: dict[str, str] = {}
    for gid, definition in definitions.items():
        head = re.split(r"[.;:\"]", definition, maxsplit=1)[0].strip()
        if len(head) < 40:
            continue
        if head in first_sentence:
            fail(f"cuasi-duplicado en glosario: {gid} y {first_sentence[head]} comparten definición inicial")
        first_sentence[head] = gid

    return {"count": len(ids), "ids": sorted(ids), "related_total": sum(len(e) - 1 for e in items),
            "dangling": dangling, "categories": {c: category_entries[c] for c in sorted(categories) if c}}


def audit_labs() -> list[dict]:
    src = read("LabScenarios.cs")
    clean = strip_noise(src)
    items = []
    ids: dict[str, int] = {}
    for m in re.finditer(r"\x00(lab-\d+)\x01", clean):
        ids[m.group(1)] = ids.get(m.group(1), 0) + 1
    for lid, n in ids.items():
        if n > 1:
            fail(f"id de laboratorio duplicado: {lid} ({n} veces)")
        items.append({"id": lid})
    return items


# -------------------------------------------------------------------- main
def main() -> int:
    lessons = audit_lessons()
    interviews = audit_interviews()
    scenarios = audit_scenarios()
    labs = audit_labs()
    glossary = audit_glossary()

    inventory = {
        "generated_from": "src/RutaTI30.Application/Content",
        "lessons": lessons,
        "interview_questions": interviews,
        "ticket_scenarios": scenarios,
        "lab_scenarios": labs,
        "glossary": glossary,
        "summary": {
            "lessons": len(lessons),
            "lesson_questions": sum(len(l["questions"]) for l in lessons),
            "interview_questions": len(interviews),
            "ticket_scenarios": len(scenarios),
            "lab_scenarios": len(labs),
            "min_words_per_day": min((l["words"] for l in lessons), default=0),
            "min_questions_per_day": min((len(l["questions"]) for l in lessons), default=0),
        },
        "failures": FAILURES,
    }
    REPORTS.mkdir(exist_ok=True)
    (REPORTS / "content-inventory.json").write_text(
        json.dumps(inventory, ensure_ascii=False, indent=2), encoding="utf-8")

    lines = [
        "# Inventario de contenido educativo — Ruta TI 30",
        "",
        "> Generado por `scripts/verify/edu-content-audit.py` desde el código fuente.",
        "",
        "## Resumen",
        "",
        "| Recurso | Cantidad |",
        "|---|---|",
        f"| Lecciones (días) | {len(lessons)} |",
        f"| Preguntas de evaluación | {sum(len(l['questions']) for l in lessons)} |",
        f"| Preguntas de entrevista | {len(interviews)} |",
        f"| Escenarios de tickets | {len(scenarios)} |",
        f"| Laboratorios | {len(labs)} |",
        "",
        "## Lecciones",
        "",
        "| Día | Bloque | Título | Dificultad | Min | Palabras | Preguntas | Estado |",
        "|---|---|---|---|---|---|---|---|",
    ]
    for l in sorted(lessons, key=lambda x: x["day"]):
        lines.append(f"| {l['day']} | {l['block']} | {l['title'][:60]} | {l['difficulty']} | "
                     f"{l['minutes']} | {l['words']} | {len(l['questions'])} | auditado |")
    lines += ["", "## Preguntas de entrevista", "",
              "| Id | Área | Tipo | Enunciado | Respuesta (palabras) | Estado |",
              "|---|---|---|---|---|---|"]
    for q in interviews:
        lines.append(f"| {q['id']} | {q['area']} | {q['kind']} | {q['statement'][:70]} | "
                     f"{q['model_words']} | auditado |")
    lines += ["", "## Escenarios de tickets", "",
              "| Id | Título | Categoría | Impacto | Urgencia | Preguntas | Pasos | Solución | Docs |",
              "|---|---|---|---|---|---|---|---|---|"]
    for s in scenarios:
        lines.append(f"| {s['id']} | {s['title'][:50]} | {s['category']} | {s['impact']} | "
                     f"{s['urgency']} | {s['diagnostic_questions']} | {s['steps']} | "
                     f"{'sí' if s['has_solution'] else 'NO'} | {'sí' if s['has_documentation'] else 'NO'} |")
    lines += ["", "## Laboratorios", ""]
    lines += [f"- {l['id']}" for l in labs]
    lines += ["", "## Resultado de la auditoría", ""]
    if FAILURES:
        lines += [f"**{len(FAILURES)} incidencia(s):**", ""] + [f"- {f}" for f in FAILURES]
    else:
        lines.append("**Sin incidencias**: todas las validaciones duras pasaron.")
    (REPORTS / "content-inventory.md").write_text("\n".join(lines) + "\n", encoding="utf-8")

    print(f"LECCIONES: {len(lessons)} | PREGUNTAS: {sum(len(l['questions']) for l in lessons)} | "
          f"ENTREVISTAS: {len(interviews)} | ESCENARIOS: {len(scenarios)} | LABS: {len(labs)} | "
          f"GLOSARIO: {glossary['count']}")
    if FAILURES:
        print(f"FAILURES: {len(FAILURES)}")
        for f in FAILURES:
            print("  - " + f)
        return 1
    print("AUDIT: PASSED")
    return 0


if __name__ == "__main__":
    sys.exit(main())
