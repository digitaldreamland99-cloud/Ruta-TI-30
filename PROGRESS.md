# PROGRESS — Ruta TI 30

> Actualizado: 2026-10-09 (sesión de finalización). Este archivo refleja solo trabajo realmente realizado.

## Fase de excelencia (segunda sesión, 2026-10-09)

Trabajo sobre la base existente, sin reconstruir nada y sin tocar la arquitectura (C#/.NET 10/WPF/MVVM/SQLite):

1. **Simulador de Help Desk — casos de práctica (B2)**: nuevo `src/RutaTI30.Application/Content/TicketPracticeScenarios.cs` con **9 escenarios L1** (cuenta bloqueada, aplicación con error, equipo lento, impresora, sin internet, DNS, DHCP/APIPA, permisos, escalamiento obligatorio) que incluyen descripción del usuario, preguntas de diagnóstico, pasos sugeridos, solución/escalamiento y documentación del ticket. Integración en `TicketsViewModel` (selector + `LoadPracticeCaseCommand` + guía) y en `WorkTemplates.xaml` (carga en el formulario + panel de guía). Los datos ficticios se etiquetan como tales.
2. **Borrado de vacantes con confirmación (Fase C)**: `IJobStore.DeleteApplication` con transacción y cascada (postulación + seguimientos) en `JobStore`, comando con diálogo de confirmación y mensaje de cancelación en `CareerViewModel`, botón «Eliminar» en `WorkTemplates.xaml`.
3. **Preparación laboral (B4)**: nueva pregunta **q21** (comunicación de la solución al usuario → banco **21** preguntas) y **guía de fortalezas/temas por repasar** (`RebuildInterviewGuide`) calculada solo desde autoevaluaciones reales, con advertencia de subjetividad; ahora se recalcula **al instante** al guardar cada respuesta (antes quedaba desactualizada).
4. **Pruebas nuevas (Fase D)**: `tests/.../Application/PracticeScenarioTests.cs` (5 pruebas) y 3 pruebas de borrado en cascada en `JobStoreTests` → **15 archivos de pruebas**.
5. **Verificación estática de cierre (Fase D/F)**: léxico **67/67** `.cs`, XML **15/15**, bindings **228/228**, contenido 30 días/120 preguntas/21 entrevistas/9 casos (ids únicos), sin llamadas de red, links `.md` íntegros — todo aprobado (TESTING.md §1).

Bloqueo externo confirmado una sola vez y no simulado: Linux sin `dotnet`/`pwsh`/`iscc`/`wine` → compilación, suite, publicación e instalador siguen **pendientes en Windows** (VERIFY.md).

## Última tarea terminada (sesión de finalización)

Trabajo ejecutado sobre la base existente, sin recrear nada:

1. **Auditoría**: DI/constructores/namespaces/catálogos verificados contra el código real; SDK ausente (comprobado una vez); sin marcadores TODO/NotImplemented en el código.
2. **Contenido educativo completado**: los 30 días alcanzan paridad de profundidad (hoy: ≥780 palabras y 4 preguntas por día; mediana 813 vs. 672 previa; 120 preguntas en total con explicación). Enriquecimiento por lección: 4.ª pregunta, conceptos, pasos de procedimiento, errores frecuentes y ejercicios guiados expandidos (días 8–30).
3. **Defectos corregidos en contenido**: id `d2q2x` → `d2q4` con enunciado completo (día 2), typo «congestelamientos» → «congelamientos».
4. **Validación de vacantes en dominio**: nueva `JobRules.Validate` (empresa/puesto obligatorios, enlace http(s), coherencia de fechas) usada por el ViewModel en alta y edición + `Domain/JobRulesTests.cs` (9 pruebas).
5. **Instalador**: configuración reproducible con Inno Setup 6 (`installer/rutati30.iss` per-user sin admin, conserva datos en desinstalación) + `scripts/build-installer.ps1` (verifica artefacto publicado y ISCC.exe, propaga códigos de salida) + INSTALLATION.md §6 y DECISIONS.md D11.
6. **Revisión de seguridad**: SQL solo parametrizado (48 sentencias / 229 referencias `@`), sin secretos incrustados, sin `HttpClient`/`Process.Start`, 2 catches vacíos justificados (comentario añadido al de FileLogger).
7. **Banco de entrevistas ampliado**: 14 → 20 preguntas (q15–q20) cubriendo las áreas que faltaban (Soporte de aplicaciones ×2, Escalamiento, Windows/Visor de eventos, Gestión del tiempo, Seguridad e ingeniería social); 12 áreas cubiertas en total, balance léxico verificado, ids únicos.
8. **Cierre definitivo (sesión final)**: comprobación única del entorno (Linux x86_64; `dotnet`, `pwsh`, `iscc`, `wine` ausentes → validación de Windows imposible aquí), verificación de correspondencia de rutas de los 4 scripts con el repositorio real, y creación de **VERIFY.md** con el procedimiento de validación en Windows (comandos, resultados esperados y casillas pendientes).

## Verificaciones ejecutadas (resultado real, sesión de finalización)

| Verificación | Resultado |
|---|---|
| Balance léxico de los 65 `.cs` (cadenas/comentarios/interpolaciones) | **Aprobado** (65/65) |
| Contenido: 30 días, palabras/preguntas por día, ids únicos | **Aprobado** (30 días únicos; 120 preguntas; mínimo 782 palabras; 0 strings vacíos) |
| Seguridad (SQL parametrizado, secretos, red, catches) | **Aprobado** |
| `dotnet build` / `dotnet test` | **No ejecutados** (SDK ausente; no se simulan) |

## Archivos creados y modificados en esta sesión (finalización)

- `src/RutaTI30.Application/Content/LessonsBloque1..5.cs` — enriquecimiento de profundidad (24 reemplazos por bloque aprox.) + corrección d2q4.
- `src/RutaTI30.Domain/Career.cs` — añadida `JobRules`.
- `src/RutaTI30.App/ViewModels/CareerViewModel.cs` — validación con `JobRules` en alta y edición.
- `src/RutaTI30.App/Services/FileLogger.cs` — comentario en catch justificado.
- `installer/rutati30.iss`, `scripts/build-installer.ps1` — nuevos.
- `tests/RutaTI30.Tests/Domain/JobRulesTests.cs` — nuevo (9 pruebas).
- Docs actualizadas: TESTING, PROGRESS, CHANGELOG, ROADMAP, INSTALLATION, DECISIONS.

Totales actuales: 65 archivos `.cs`, 8 XAML, 14 archivos de pruebas + helpers, 11 documentos `.md`, 4 scripts `.ps1`, 1 `.iss`.

## Pruebas y verificaciones ejecutadas (resultado real)

| Verificación | Herramienta | Resultado |
|---|---|---|
| `dotnet --info` / `--list-sdks` | terminal | **Fallido**: `dotnet: command not found` (entorno sin SDK .NET) |
| `dotnet build` / `dotnet test` | terminal | **No ejecutados** (bloqueo de entorno; no se simulan) |
| Bien-formed XML de los 8 XAML + 6 csproj/props | python3 xml.dom.minidom | **Aprobado** (14/14) |
| Balance léxico de llaves/paréntesis/corchetes en los 64 `.cs` (con análisis de cadenas/comentarios/interpolaciones) | script python3 | **Aprobado** |
| Todo `{Binding X}` de las vistas resuelve a una propiedad/registro real de los ViewModels | script python3 | **Aprobado** |
| Corrección de artefactos detectados y corregidos durante la revisión | ediciones + re-verificación | Corregidos: flechas `→` inválidas en C#, colisiones de nombres `LessonState`/`TimerStatus`, paréntesis rotos en `InterviewQuestions.cs`, índices de columna XAML, `StringFormat` con llaves sin escapar, `SqliteConnection.ClearAllPools()` añadido tras restaurar, typos de contenido, referencia inexistente `InterviewBankTests.cs` en TESTING.md (sustituida por `ContentTests.cs`) |
| Re-verificación al cierre de sesión (2026-10-09): XML 15/15, balance léxico 64/64 `.cs`, resolución de 184 `{Binding}` de vistas | python3 | **Aprobado** |

## Bloqueos comprobados

1. **SDK .NET ausente** en el entorno Linux actual → compilación, `dotnet test` y ejecución WPF quedan **pendientes en Windows con .NET 10 SDK**. Todo lo demás se implementó y verificó estáticamente.
2. Versiones de paquetes NuGet son **candidatas** (DECISIONS.md D8); confirmar con `dotnet restore` en Windows y ajustar si hace falta.
3. No hay instalador: requiere elegir y verificar una herramienta de instalación en Windows (documentado en INSTALLATION.md; R-23 pendiente).

## Próxima tarea exacta (para quien retome con SDK disponible)

1. `powershell -ExecutionPolicy Bypass -File scripts\build.ps1` → corregir errores de compilación si aparecen.
2. `powershell -ExecutionPolicy Bypass -File scripts\test.ps1` → aprobar/corregir la suite (14 archivos, incluye JobRulesTests); actualizar TESTING.md con resultados reales.
3. Ejecutar la app y verificar a mano: navegación de 7 módulos, persistencia tras reinicio, temporizador (incluida recuperación tras cierre), tickets (transición inválida debe rechazarse), validación de vacantes (enlace no-http y fechas incoherentes deben rechazarse), evaluaciones, respaldo/restauración con archivo inválido (debe rechazarse), funcionamiento sin red.
4. Cambiar estados de REQUIREMENTS.md de `SV` a `V` solo con evidencia de esas ejecuciones.
5. Publicar win-x64 con `scripts\publish.ps1`, instalar Inno Setup 6, ejecutar `scripts\build-installer.ps1` y probar instalación/desinstalación en VM limpia (verificar que los datos de `%LOCALAPPDATA%\RutaTI30` sobreviven a la desinstalación).
