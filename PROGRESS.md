# PROGRESS — Ruta TI 30

> Actualizado: 2026-10-13 (auditoría integral final). Este archivo refleja solo trabajo realmente realizado.

## Auditoría integral final — PROMPT MAESTRO DEFINITIVO (2026-10-13)

Auditoría sin editar código (Etapa A), contraste de las 14 capacidades objetivo contra la implementación real:

| # | Capacidad objetivo | Estado | Evidencia |
|---|---|---|---|
| 1 | Saber dónde quedé | **Cumple** | Día académico persistido (`preferences.program.current_day`, cambia solo por acción explícita) + título/objetivo de la lección del día en Inicio |
| 2 | Qué estudiar | **Cumple** | Temas débiles (`ProgressService` desde `attempts.details_json`), actividades pendientes según estado real de la lección, repaso inteligente (`ReviewService`: 1/2/4/8/14 y 3/7/14/30 días) |
| 3 | Continuar actividad | **Cumple** | `ContinueCommand` → programa de estudios + `OpenModuleCommand` con 7 accesos rápidos a módulos reales |
| 4 | Conceptos desde cero | **Cumple** | 30 lecciones/120 preguntas validadas por el auditor de contenido; explicación, ejemplo laboral, procedimiento, errores frecuentes |
| 5 | Escenarios realistas | **Cumple** | 10 laboratorios con decisiones ponderadas (`LabScoring`), 16 casos de práctica Help Desk con guía y acciones simuladas (el SO nunca se toca) |
| 6 | Tickets simulados | **Cumple** | Help Desk completo: CRUD, transiciones validadas, historial, prioridad impacto×urgencia (`TicketRules`), búsqueda literal |
| 7 | Explicaciones aciertos/errores | **Cumple** | Retroalimentación por pregunta + soluciones explicadas + resultado del caso con criterios |
| 8 | Repasar lo no dominado | **Cumple** | Cola de repaso solo desde intentos reales; historial de intentos conservado |
| 9 | Practicar entrevistas | **Cumple** | 34 preguntas en 17 áreas con respuesta modelo/explicación/errores/criterios; guía de fortalezas solo desde autoevaluaciones guardadas |
| 10 | Mapa de competencias | **Parcial** | Progreso lista «temas débiles» y «competencias demostradas» con cálculo real; falta la matriz formal por competencia (descripción/ejercicios/evidencias/estado/next step) — registrado en P2 |
| 11 | Persistencia fiable | **Cumple (pendiente Windows)** | 6 almacenes SQLite locales; 23 archivos de pruebas escritos (no ejecutables aquí) |
| 12 | Offline | **Cumple (verificado estátamente)** | 0 llamadas de red en `src/`; datos en `%LOCALAPPDATA%\RutaTI30` |
| 13 | Recuperación ante errores | **Cumple** | `BackupService`: copia validada, restauración con copia de seguridad previa + refresco de módulos vía `IDbStateChanged`; copia corrupta rechazada (pruebas escritas) |
| 14 | Interfaz consistente | **Cumple (pendiente Windows)** | Kit visual compartido (misiones 3.0–6.0), semántica nominal, foco/hover/tooltip/estados vacíos |

**Matriz de brechas priorizada (resultado de la auditoría):**

- **P0**: ninguna nueva detectada. Los P0 históricos (llave faltante en prueba, `NullDbStateChanged` inexistente, F-05 sin oyente) quedaron corregidos en misiones 2.x con evidencia. Los defectos de compilación NO pueden confirmarse ni descartarse aquí (sin SDK; limitación registrada).
- **P1**: (a) compilación + suite xUnit — bloqueo de entorno, no de código; (b) «Diagnóstico inicial» y «Mapa de competencias formal» — no implementados **de forma deliberada**: requieren servicios/ViewModels nuevos de C# y sin compilador disponible un error de tipo/namespace pasaría los verificadores estáticos y rompería la compilación en Windows (regresión silenciosa). Registados como PROPUESTOS, no escritos a ciegas.
- **P2**: consolidar `BoolToVis` en recurso compartido (hallazgo menor preexistente); enriquecer cierres de casos de práctica con «qué hizo bien / mal / alternativa preferible» sin escribir lógica secundaria nueva.
- **P3**: `EnableWindowsTargeting=true` en csproj de pruebas (solo CI Linux); medición del tick 1×/s de Inicio antes de cualquier optimización.

**Prueba funcional estática (protocolo §7.5)**: 0 de 42 botones de las vistas sin `Command` (análisis multilínea con Python; el conteo por línea anterior era un fallo de la heurística) y 0 `ListBox` sin `ItemsSource` (8 falsos positivos eran subetiquetas `ItemTemplate`/`ItemContainerStyle`). No hay acciones huérfanas; `NavItems` = los 9 módulos reales.

## Auditoría lingüística y de codificación (2026-10-13, continuación de la auditoría integral)

1. **Barridos sin hallazgos**: 0 caracteres corruptos (UTF-8/Unicode) en `src/` y `tests/` fuera de los signos españoles legítimos; 0 tildes faltantes en palabras frecuentes; 0 dobles espacios en `Text/Content/ToolTip` de las vistas; 0 palabras duplicadas en cadenas XAML.
2. **Correcciones lingüísticas mínimas (2, ambas de redacción, sin tocar respuestas/puntuación)**:
   - `LessonsBloque4.cs` (lección IAM): «retirar TODO al salir» → «retirar todo al salir» — el texto pedía mayúsculas que lo hacían atractivo como marcador de código (`TODO`); ahora se lee de forma natural.
   - `LabScenarios.cs:29` (lab-01): «entre sí sí se comunican» → «sí se comunican entre sí» — grammaticalmente válida pero ambigua para principiantes; mismo significado, contexto de escenario y sin alterar opciones ni respuestas.
3. **Verificación tras las correcciones**: static-regression PASSED (exit 0), edu-content-audit PASSED (exit 0, conteos 30/120/34/16/10/59 intactos), `git diff --check` limpio; segunda revisión con grep confirmó que no quedan más coincidencias de los patrones corregidos.
4. **Archivos modificados en esta pasada**: solo los dos fuente arriba citados + docs aditivos (TESTING.md, VERIFY.md, este archivo). Los otros 55 cambios de estado del árbol permanecen intactos y sin reconocidades por novedades.

## Misiones 7.0–9.0 — Auditoría, validación y publicación (2026-10-13)

1. **Inspección inicial**: `main`/`9afd8ad`, sin commits/push, 55 entradas de estado intactas, `961d2fb` tipo `commit` intacto, herramientas Linux: Python ✓ y `dotnet/pwsh/iscc/wine/msbuild` ausentes; scripts de distribución presentes (`installer/rutati30.iss`, `scripts/{build,test,publish,build-installer}.ps1`) sin entorno para ejecutarlos. Línea base: 4 verificadores con exit 0.
2. **Misión 7.0 — auditoría con evidencia**: strings reales confirmados en código (`LearnViewModel.StateLabel` → «No iniciado/En curso/Completado/Dominado»; `TicketsViewModel` → «P1 · Crítica/P2 · Alta/P3 · Media/P4 · Baja» y `TicketRules.CalculatePriority` mapa impacto×urgencia). **Hallazgo demostrado y corregido**: las insignias del estado de lección (Ruta) y de prioridad de ticket (Help Desk) seguían usando `AccentBrush` como fondo (estado pintado con color de acción). Corrección: mismo patrón semántico aprobado en 6.0 — texto nominal sobre superficie neutra con `DataTriggers` (P1=Danger, P2=Warning, P3=Accent, P4=Muted); cero cambios de ViewModel; los DataTriggers usan los strings exactos producidos por los ViewModels.
3. **Misión 8.0 — validación**: verificadores estáticos re-ejecutados tras las correcciones: static-regression PASSED (XML 16/16, léxico 81/81, bindings 232/232 con los 6 DataTriggers nuevos sin bindings añadidos — los cambios son solo visuales), edu-content-audit PASSED, d09 PASSED, `git diff --check` limpio. Compilación/pruebas xUnit/validación visual: **BLOQUEADAS** (sin .NET 10/PowerShell/Windows; no simuladas). Rendimiento: sin medición disponible; hypotheses previas documentadas, no optimizadas a ciegas. Opción menor registrada sin implementar: `EnableWindowsTargeting=true` en el csproj de pruebas para compilación cruzada en CI Linux (el propio comentario del archivo lo menciona; no es defecto en Windows).
4. **Misión 9.0 — publicación**: estrategia inspeccionada (`publish.ps1` → `artifacts\win-x64\RutaTI30.exe` self-contained; `build-installer.ps1` → Inno Setup → `artifacts\installer\rutati30-0.1.0-win-x64-setup.exe` per-user que conserva datos). **No se generó ningún artefacto** (0 archivos `*.exe`/`*.msi`/`*setup*`); las instrucciones exactas para Windows ya están en VERIFY §3–§4 e INSTALLATION.md.
5. **Archivos de estas misiones**: `src/RutaTI30.App/Views/ModuleTemplates.xaml` (insignia de estado de lección semántica), `src/RutaTI30.App/Views/WorkTemplates.xaml` (insignia de prioridad semántica), y docs aditivos (este archivo, TESTING.md, VERIFY.md). Cero cambios en ViewModels, servicios, esquema SQLite, respaldos, NuGet o red.

## Misión 6.0 — Rediseño premium integral (2026-10-13)

1. **Fase Cero**: reconciliación real confirmada: `main`/`9afd8ad`, sin commits/push, 55 entradas de estado intactas, `961d2fb` tipo `commit` intacto, `dotnet`/`pwsh` ausentes (entorno Linux, contraste documentado). Línea base: los 4 verificadores con exit 0.
2. **Fase A (auditoría)**: matriz de las 9 secciones ya consolidada en misiones 3.0–5.0 (Inicio/dashboards accesos rápidos ok; Ruta con opciones de evaluación estilizadas; Laboratorio con ancho de lectura y resultado con mensaje; Tickets validados en 4.0 sin tocar lógica; Preparación laboral con lectura 760 ahora; Progreso con temas débiles estilizados; Glosario con búsqueda/filtro y lectura ok; Rutas listadas de forma consistente; Configuración con horario/tema/datos). Hallazgo menor preexistente documentado: `BoolToVis` se define solo en `HomeTemplates.xaml` y se resuelve a nivel de aplicación (funciona; consolidarlo sería mejora futura opcional).
3. **Fase B (sistema de diseño)**: `Styles.xaml` + entrada discreta de mensajes (180 ms de opacidad al volverse visible, estilo `MsgBase` del que dependen `MsgInfo/Success/Warning/Error`) y transición de press 120 ms (solo entrada, sin segunda animación de salida sobre la misma propiedad para evitar storyboards simultáneos; el estado final es idéntico si las animaciones se desactivan).
4. **Fase C (dashboard)**: la insignia de estado del día ya no usa el color de acción como fondo: ahora usa texto semántico sobre superficie neutra (No iniciado = gris local, En curso = acento, Completado/Dominado = éxito), con DataTriggers en vista (cero cambios de ViewModel) y texto que nombra la condición (no depende del color).
5. **Fases D–E (vistas y microinteracciones)**: `MsgInfo` aplicado a las tres tarjetas de resultado (bienvenida Inicio, evaluación de lección, resultado del caso de laboratorio, todas con animación de entrada discreta); lectura a 760 px en respuestas modelo y explicaciones de entrevistas que las tenian.
6. **Fase F (rendimiento)**: sin cambios: sin sistema de medición disponible aquí; hypothesis previa (tick 1×/s de Inicio) sigue registrada como pendiente de medición, no optimizada a ciegas.
7. **Fase G (accesibilidad)**: la insignia semántica mejora la diferenciación sin depender del color; foco de teclado/hover/tooltip del kit intactos y validados estáticamente.
8. **Fase I (verificación)**: static-regression PASSED (XML 16/16, léxico 81/81, bindings 226/226 tras 3 bindings nuevos de la insignia, sin red, SQL parametrizado, exit 0); auditoría de recursos: 19 claves usadas en Styles, 0 faltantes, 0 `StaticResource` fuera de orden, XML de vistas bien formado; `git diff --check` limpio (exit 0).
9. **Fase J (publicación) BLOQUEADA**: mismo bloqueo de entorno (sin .NET 10/PowerShell/ISCC); nada simulado.
10. **Archivos de esta misión**: `src/RutaTI30.App/Themes/Styles.xaml` (kit ampliado), `src/RutaTI30.App/Views/HomeTemplates.xaml` (estado semántico + MsgInfo bienvenida), `src/RutaTI30.App/Views/ModuleTemplates.xaml` (MsgInfo evaluación/caso), `src/RutaTI30.App/Views/WorkTemplates.xaml` (lectura 760), y docs aditivos.

## Misión 5.0 — Validación final (2026-10-13)

1. **Fase 1 (diagnóstico) — APROBADA**: rama `main`, HEAD `9afd8ad`, sin commits/push; 33 modificados + 22 sin seguimiento preservados; objeto `961d2fb` intacto (tipo `commit`), `git stash list` vacío (preexistente); 0 artefactos previos (`find *.exe|*.msi|*setup*` → 0); herramientas comprobadas: Python 3.10 ✓, `dotnet/pwsh/powershell/iscc/wine/msbuild/mono` ausentes (evidencia directa, no cierta suposición); TESTING.md y REQUIREMENTS.md re-leídos en su estado vigente.
2. **Fases 2–5 (compilación → pruebas xUnit → validación visual WPF → publicación/instalador) — BLOQUEADAS por entorno, NO EJECUTADAS ni simuladas**: Linux 6.1.158+ x86_64 sin .NET 10 SDK ni PowerShell; `dotnet --version` → «command not found»; `pwsh -File scripts/test.ps1` → «pwsh: command not found». Las casillas §1–§4 de VERIFY.md permanecen «Pendiente» para Windows.
3. **Fase 6 (cierre) — APROBADA (estática)**: `static-regression.py` PASSED (XML 16/16, léxico 81/81, bindings 223/223, sin red, SQL parametrizado, exit 0); `edu-content-audit.py` PASSED (30/120/34/16/10/59, exit 0); `d09-wire-foundations-verification.py` exit 0; `git diff --check` limpio (exit 0); diff global confirmado (33 archivos, +1051/−69): nada del trabajo previo se perdió; `Styles.xaml`, `HomeViewModel`, `HomeTemplates`, `ModuleTemplates` y `GlossaryTemplates` contienen las mejoras de la Misión 4.0.
4. **Sin cambios de código en esta misión** (rol de validación): cero modificaciones en src/, tests/ y scripts/; solo documentación aditiva (TESTING.md, VERIFY.md, PROGRESS.md).
5. **Próximo paso**: ejecutar en Windows 10/11 con .NET 10 SDK las fases 2–5 con los comandos exactos de VERIFY.md (scripts\build.ps1 → scripts\test.ps1 → recorrido visual §2 → scripts\publish.ps1 → scripts\build-installer.ps1), corrigiendo solo defectos demostrados y actualizando las casillas con evidencia real.

## Misión 4.0 — Modernización integral (2026-10-13)

1. **Fase Cero (reconciliación)**: rama `main`, HEAD `9afd8ad`, sin commits/push nuevos. Árbol con todo el trabajo previo intacto (29 modificados + ~20 sin seguimiento). Objeto `961d2fb` confirmado **sin historial alterado**: tipo `commit` («WIP on main: 9afd8ad…», 2 padres), `git stash list` vacío como preexistente. Entorno reconfirmado: Python 3.10 ✓; `dotnet`, `pwsh`, `iscc` ausentes → compilación/WPF/instalador siguen pendientes en Windows (no simulados). Verificadores base ejecutados con exit 0: static-regression, edu-content-audit, d09-wire-foundations.
2. **Etapa C (dashboard)**: `HomeViewModel` + un comando read-only `OpenModuleCommand` (reutiliza el mecanismo `Navigate` existente que `MainViewModel` ya asigna; sin nuevos servicios ni dependencias) y `HomeTemplates.xaml` + nueva tarjeta «Accesos rápidos» con 7 botones reales que navegan a los módulos existentes (lab, tickets, career, progress, glossary, routes, settings), iconos nativos con el estilo `Icon` en Mi progreso/Actividades pendientes/Temas de repaso y subtítulo «Derivados de tu historial de evaluaciones». Nada de métricas nuevas sin fuente fiable; se reutilizan `CountsText`, `StudyToday/Total`, `WeeklySummary`, `LastAttemptText`. Máximo conservado: bienvenida, mi día, horario y temporizador intactos.
3. **Etapa D (pantalla piloto: lección/cuestionario)**: en la plantilla de la ruta de aprendizaje se fijó ancho máximo de lectura (760 px) en Explicación y Ejemplo laboral, y las opciones de respuesta tienen plantilla propia con estado visible (hover + borde de acento al seleccionar). No se cambió la lógica de evaluación, historial ni puntuación.
4. **Etapas E–G (extensiones y accesibilidad)**: ancho de lectura aplicado también a Glosario (definición sencilla/técnica y ejemplo laboral) y al contexto del laboratorio. En accesibilidad, los recursos ya validados en la Etapa B cubren el kit (FocusRing en botones, foco visible en campos, tooltips temáticos, hover/pressed/disabled). Limitación documentada: la preferencia de reducción de movimiento de Windows no se ha marcado en el código, por lo que no se declara cumplimiento (el hover de 150 ms es opcional: mismo estado final si se desactiva). Rendimiento: sin cambios de código no medibles; queda como preventivo el tick 1×/s del reloj de Inicio (bajo costo constatado por lectura, no medido).
5. **Etapa H (calidad educativa)**: sin modificaciones de contenido — no se detectaron defectos de exactitud con evidencia suficiente en esta revisión; no se agregó contenido por cuota (regla de la misión).
6. **Validación real por etapa (exit codes)**: static-regression PASSED y `git diff --check` limpio tras C, D y E; al cierre: XML 16/16, léxico 81/81, bindings 223/223 (7 command-bindings nuevos resueltos), sin red, SQL parametrizado. `edu-content-audit` y `d09-wire-foundations` retenidos en exit 0 previo a ediciones (no editaron .cs/.contenido en esta misión).
7. **Persistencia intacta**: cero cambios en Infrastructure/ (esquema, migraciones, respaldos), cero SQL nuevo, cero paquetes NuGet, cero llamadas de red.
8. **Pendiente en Windows**: render visual de los 2 temas, foco/hover/tooltips en pantalla, navegación de los 7 accesos rápidos, compilación y suite xUnit (23 archivos), instalación del paquete (VERIFY §1–§4).

## Misión 3.0 — Modernización visual (etapas A y B, 2026-10-13)

1. **Estado preservado**: rama `main`, HEAD `9afd8ad`, todo el trabajo pendiente previo intacto (sin commits ni push). El objeto commit `961d2fb` sin tocar.
2. **Etapa A (diagnóstico, sin edición)**: inventario del sistema visual real (`Themes/Dark|Light|Styles.xaml`, 5 diccionarios de plantillas, 9 módulos, DI en `App.xaml.cs`, `DashboardService` con 100 % datos reales), 10 problemas UX/UI documentados (hover por opacidad brusca, sin anillo de foco, campos sin estado de foco, insignias semánticamente ambiguas, sin kit de estados de mensaje/vacío, tooltip nativo claro sobre tema oscuro, navegación sin indicador, sin microinteracciones, líneas de lectura sin límite de ancho, tick 1×/s de Inicio) y plan por etapas B→G con riesgo y alcance. No se modificó ningún archivo durante la fase de diagnóstico.
3. **Etapa B (sistema visual)**: solo la primera etapa autorizada por el plan. Archivo modificado: `src/RutaTI30.App/Themes/Styles.xaml`. Añadidos: tokens `RadiusSm/RadiusMd`; `FocusRing` (anillo de foco por teclado en `BtnBase`); foco visible en `Field` (`IsKeyboardFocusWithin` → borde de acento, sin tocar el valor del texto); transición de hover 150 ms en botones (`DoubleAnimation` de Opacity con valor final idéntico al setter: si las animaciones se desactivan, las funciones quedan intactas); indicador de sección activa en `NavList`; variantes de insignia `BadgeSuccess/Warning/Danger/Accent` + textos `BadgeText*`; mensajes `MsgInfo/Success/Warning/Error`; `BarSuccess`; estado vacío `EmptyState/EmptyStateText`; tooltip con plantilla del tema; estilo `Icon` (Segoe MDL2 Assets, fuente de Windows, sin dependencias).
   **No se renombró ni eliminó ninguna clave existente**; no hay sombras blur (`DropShadowEffect`) por rendimiento; no se tocaron ViewModels, servicios, esquema SQLite ni contratos.
4. **Defecto corregido durante la própria etapa**: el trigger de foco de `Field` referenciaba `TargetName="bd"` sin que el `Border` tuviera `x:Name`; corregido y re-verificado (el XML no habría cargado).
5. **Validación real (exit codes comprobados)**: `static-regression.py` → XML 16/16, léxico 81/81 `.cs`, bindings 216/216, sin red, SQL parametrizado, `STATIC_REGRESSION: PASSED` (exit 0); `git diff --check` → limpio (exit 0); auditoría ad hoc → 18 claves de recurso usadas, 0 faltantes, 0 `StaticResource` fuera de orden de definición, XML de `Styles.xaml` bien formado.
6. **Pendiente en Windows (no simulado)**: render de los 2 temas con los recursos nuevos (casillas 1.1–1.3 de VERIFY), recorrido visual de las 9 secciones (§2), y las etapas C–G del plan.

## Misión 2.2 — Preparación de validación en Windows (2026-10-12)

1. **Estado preservado**: cambios de Misión 2.1 presentes (`StudyTimerTickIntegrationTests.cs`, TESTING/VERIFY/PROGRESS). Sin commits ni push.
   **Precisión sobre el stash**: `git stash list` está vacío (no existe el ref `refs/stash`; fue eliminado antes de estas sesiones — ya era así al inicio de la Misión 2.1), pero el **objeto commit** `961d2fb` («WIP on main: 9afd8ad…», 2 padres, forma clásica de stash) sigue existiendo en la base de objetos y `git stash show --stat 961d2fb` funciona: contiene el trabajo de entrevistas/escenarios **ya recuperado al árbol**. Nada de estas sesiones lo modificó. Si se desea conservarlo a prueba de `gc`, quien tenga Windows puede ejecutar `git stash store -m "961d2fb" 961d2fb` (decisión del usuario; aquí no se tocó para mantenerlo intacto).
2. **Scripts auditados** (`build.ps1`, `test.ps1`, `publish.ps1`, `build-installer.ps1`): rutas válidas (Set-Location a la raíz), códigos de salida propagados, Debug+Release definidos, no eliminan datos de usuario, pruebas sobre `RutaTI30.sln` (que incluye los 5 proyectos, Tests incluido). Sin correcciones necesarias.
3. **Compatibilidad**: TFMs consistentes (`net10.0` en Domain/Application/Infrastructure; `net10.0-windows`+`UseWPF` en App/Tests); referencias entre proyectos correctas; paquetes 10.0.0 / Test.Sdk 17.14.1 / xunit 2.9.3 siguen como **candidatos** pendientes de `dotnet restore` (DECISIONS.md D8).
4. **Defecto documental corregido**: TESTING.md tenía filas duplicadas de Misión 2.1 con **nombres de API inexistentes** (`EvaluateAttemptAsync`, `PrepareDescription`; verificado con grep: 0 coincidencias). Deduplicadas y reemplazadas por APIs reales (`EvaluationService.Evaluate`, `AppDb.DatabasePath`). Conteo de suite corregido en VERIFY 1.2: **23 archivos de prueba** (no 18); §3 de TESTING completado con los 5 archivos que faltaban.
5. **Cobertura de pruebas verificada** contra la lista de la misión: inicialización de dependencias (DependencyCompositionTests), restauración y fallos de restauración (BackupServiceTests + BackupServiceIntegrationTests), notificación de BD (ambos), resultados de evaluaciones y preguntas sin responder (EvaluationServiceTests), temas débiles (ProgressWeakTopicsIntegrationTests), temporizador (StudyTimerServiceTests + TickIntegrationTests), persistencia (LearningStoreTests), búsqueda literal de tickets (TicketStoreTests con `%`/`_`/`%_`), contenido (ContentTests, PracticeScenarioTests, GlossaryTests, SpecializationRouteTests).
6. **Procedimiento de validación ampliado** (TESTING §2): SDK, restore, build Debug+Release, suite, filtrado de fallos, inicio WPF y registro de resultados.
7. **Pendiente real e inalterado**: compilación, `dotnet test` y recorrido WPF requieren Windows con .NET 10 SDK (SDK ausente aquí, comprobado). No se amplió contenido ni código productivo.

## Misión 2.1 — Auditoría de hallazgos nuevos (2026-10-12)

1. **Estado preservado**: rama `main`, stash `961d2fb` intacto (contiene el trabajo de entrevistas/escenarios ya recuperado al árbol; no se toca). Sin commits ni push.
2. **Hallazgo confirmado y corregido (P0 de compilación)**: `tests/RutaTI30.Tests/Application/StudyTimerTickIntegrationTests.cs` — la prueba `Start_Pausa_Reanudacion_YFinalizacion_PersistenSoloTiempoVerificado` no tinha la llave de cierre (el balance léxico detectaba 80/81 y static-regression fallaba con exit 1). Se añadió `}`. Cambio mínimo, sin tocar lógica de producción ni aserciones.
3. **Re-verificación real**: `static-regression.py` → 81/81 léxico PASSED (exit 0); `edu-content-audit.py` → PASSED (30 lecciones / 120 preguntas / 34 entrevistas / 16 escenarios / 10 labs / 59 términos); `d09-wire-foundations-verification.py` → PASSED; `git diff --check` → limpio.
4. **Hallazgos de la revisión externa clasificados**: referencia de pruebas → App (existe en csproj), heredabilidad de `DialogService` (correcta y necesaria para el stub), `AsyncRelayCommand.ExecuteTask` ( existe y es coherente; devuelve `Task.CompletedTask` si `CanExecute` es false)
5. **No se amplió contenido educativo ni nuevas funciones** (regla de esta misión). Pendiente real: compilación y `dotnet test` en Windows con .NET 10 SDK (SDK ausente en este entorno Linux, comprobado una vez más).

## Sesión de auditoría y corrección P0 (2026-10-11, segunda)

1. **Defecto P0 de compilación confirmado y corregido**: `BackupService` construía `NullDbStateChanged.Instance` pero la clase **no existía** en el árbol (residuo del conflicto stash/árbol de la recuperación). Se creó `NullDbStateChanged` (clase interna, singleton, implementa `IDbStateChanged`) dentro de `Infrastructure/BackupService.cs`: la restauración consigue funcionar sin suscriptor en pruebas.
2. **Notificación realmente efectiva (cierre del F-05)**: `BackupService.Restore()` ahora invoca `_dbStateChanged.Raise()` **solo cuando la restauración es exitosa** (paso 9 del flujo, tras reaplicar migraciones). Un fallo no notifica: la base sigue siendo la anterior. Cableado completo: `DbChangedNotifier` (App) registrado en DI → `BackupService` lo recibe → `Restore` lo levanta → **`MainViewModel` se suscribe a `IDbStateChanged.DataChanged` y refresca los módulos con datos cargados** (Inicio, Ruta, Laboratorio, Tickets, Preparación laboral, Progreso y Configuración; el glosario y las rutas provienen de catálogos en memoria y no requieren refresco).
3. **Pruebas xUnit nuevas de restauración en `BackupServiceTests`**: dependencia consentida sin notificador, restauración exitosa notifica exactamente una señal y restauración fallida no notifica. Documentadas en TESTING.md; ejecución de la suite pendiente en Windows (sin SDK .NET local).
4. **Auditor fortalecido** (`scripts/verify/edu-content-audit.py`): validación de categorías del glosario (no vacías) y **detección de cuasi-duplicados** (dos entradas con la misma frase inicial de definición ≥ 40 caracteres).
5. **Hallazgo real del auditor**: los 2 términos nuevos del 11-oct (Windows Update y recuperación, CMD y PowerShell) usaban la categoría «Windows» en lugar de «Windows y diagnóstico» — corregido en `Glossary.cs`; el registro con **6 categorías, 59 términos** ahora es consistente.
6. **Re-ejecución de las verificaciones disponibles** (todas con exit 0 y salida real registrada):
   `static-regression` (XML 16/16, léxico 81/81 `.cs`, bindings 216/216, sin red, SQL parametrizado),
   `edu-content-audit` (59 términos con categorías coherentes), `d09-wire-foundations-verification`, `git diff --check`.
7. Documentación actualizada: TESTING.md §1 (evidencia con comandos y resultado real), VERIFY.md §0 y §2 (nueva casilla 2.11 específica del F-05 y 2.13 para búsqueda literal en tickets), CHANGELOG.md.

## Sesión de recuperación y ampliación (2026-10-11)

1. **Trabajo perdido recuperado**: la ampliación del banco de entrevistas (21 → **34**, q22–q34) y de los casos de práctica (9 → **16**, prac-10…prac-16) existía en un *stash* de git (`961d2fb`) y **no estaba en el árbol de trabajo**. Se restauraron `InterviewQuestions.cs`, `TicketPracticeScenarios.cs`, `ContentTests.cs` y `PracticeScenarioTests.cs` desde ese stash; los documentos (README, CHANGELOG) ya anunciaban esos conteos y ahora vuelven a ser ciertos.
2. **Glosario ampliado 49 → 59 términos**: 10 entradas nuevas en tres áreas cubriendo los huecos detectados: seguridad (phishing, malware, ransomware, copia de seguridad, antivirus/EDR, actualizaciones/parches), Windows (Windows Update y recuperación, CMD y PowerShell) y preparación laboral (CV orientado a soporte, laboratorio doméstico). Enlaces entre términos y con lecciones validados por script.
3. **Auditoría de glosario integrada** en `scripts/verify/edu-content-audit.py` (ids únicos, términos relacionados existentes, conteo). Salida: `30 lecciones / 120 preguntas / 34 entrevistas / 16 escenarios / 10 labs / 59 términos de glosario`.
4. **`GlossaryTests`** endurecido: mínimo 55 entradas (antes 30).

## Sesión educativa (2026-10-10) — NOTA de corrección del 11-oct

**Corrección de honestidad:** parte del trabajo listado en esta sección (34 entrevistas, 16 escenarios, pruebas asociadas) quedó en un *stash* y no en el árbol; tras restaurarlo, los conteos vuelven a ser reales. Ver sección «Sesión de recuperación y ampliación (2026-10-11)».

Trabajo incremental sobre la base existente, sin reconstruir módulos ni tocar la arquitectura:

1. **Glosario local** (`Content/Glossary.cs`, `GlossaryViewModel`, `Views/GlossaryTemplates.xaml`, sección «Glosario» en la navegación): **49 términos** en 5 categorías (Computación y hardware 7, Windows y diagnóstico 9, Redes 15, Service Desk 9, Seguridad e IAM 9). Cada entrada: definición sencilla, explicación técnica, ejemplo laboral, error frecuente, términos relacionados y días de lección asociados (1–30). Búsqueda insensible a mayúsculas y acentos (normalización Unicode) + filtro por categoría. Catálogo inyectado (`IGlossaryCatalog`), sin duplicar contenido de lecciones.
2. **Rutas posteriores al día 30** (`Content/SpecializationRoutes.cs`, `RoutesViewModel`, `Views/RoutesTemplates.xaml`, sección «Rutas»): **5 itinerarios** (ruta-a Service Desk L1, ruta-b Soporte de aplicaciones, ruta-c Infraestructura y redes, ruta-d Windows y Microsoft 365, ruta-e IAM) con prerrequisitos, competencias, lecciones base (días existentes 1–30), prácticas (`prac-*` existentes) y áreas del banco de entrevistas; declarados opcionales, no como requisito de un primer empleo.
3. **Casos de práctica 9 → 16** (`TicketPracticeScenarios`): añadidos VPN (prac-10), correo sin sincronizar (prac-11), fallo tras actualización (prac-12), solicitud de permisos (prac-13), incidente multiusuario (prac-14), contraseña válida en un servicio pero no en una aplicación (prac-15) y equipo que no enciende (prac-16). Cobertura de las 15 situaciones pedidas en el prompt.
4. **Banco de entrevistas 21 → 34** (`InterviewQuestions`): 17 áreas cubiertas (hardware, Windows, redes, diagnóstico, Service Desk, aplicaciones, seguridad/IAM, atención al usuario, priorización, equipo, experiencia transferible, manejo de errores, límites técnicos, escalamiento, gestión del tiempo, identidades y accesos, motivación), ids únicos, estructura por helper `P(...)` con respuesta modelo, explicación, errores frecuentes, criterios y seguimientos.
5. **Repaso espaciado** (`Services/ReviewService.cs` + sección en `ProgressViewModel` y UI): cola derivada solo de `evaluation_attempts.details_json`; fallo → repaso a 1, 2, 4, 8, 14 días; aciertos consecutivos → 3, 7, 14, 30 días; temas nunca fallados no entran en la cola. Visible en Mi progreso («Repaso inteligente»).
6. **Pruebas nuevas**: `GlossaryTests.cs` (5 pruebas), `SpecializationRouteTests.cs` (4 pruebas), `ReviewServiceTests.cs` (6 pruebas); `PracticeScenarioTests` y `ContentTests` actualizadas a los conteos reales → **18 archivos de pruebas**.
7. **Verificadores estáticos reproducibles** (`scripts/verify/`): `static-regression.py` (XML 16/16, léxico 75/75 `.cs`, bindings 216/216, sin red, SQL parametrizado) y `edu-content-audit.py` (30 lecciones / 120 preguntas / 34 entrevistas / 16 escenarios / 10 labs, ids únicos e índices correctos). **Ambos aprobados** en esta sesión.
8. **Documentación**: README, USER_GUIDE, REQUIREMENTS (R-24…R-27), TESTING, VERIFY, CHANGELOG, ROADMAP, ARCHITECTURE y PROGRESS ajustados al estado real (9 secciones de navegación, conteos nuevos).

Bloqueo externo sin cambios: Linux sin `dotnet` → compilación, suite xUnit y ejecución siguen **pendientes en Windows** (VERIFY.md). No se simularon resultados.

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
2. `powershell -ExecutionPolicy Bypass -File scripts\test.ps1` → aprobar/corregir la suite (18 archivos, incluye JobRulesTests, GlossaryTests, SpecializationRouteTests y ReviewServiceTests); actualizar TESTING.md con resultados reales.
3. Ejecutar la app y verificar a mano: navegación de 9 secciones (incluye Glosario y Rutas), persistencia tras reinicio, temporizador (incluida recuperación tras cierre), tickets (transición inválida debe rechazarse), repaso espaciado (fallar una evaluación y comprobar la cola en Mi progreso), validación de vacantes (enlace no-http y fechas incoherentes deben rechazarse), evaluaciones, respaldo/restauración con archivo inválido (debe rechazarse), funcionamiento sin red.
4. Cambiar estados de REQUIREMENTS.md de `SV` a `V` solo con evidencia de esas ejecuciones.
5. Publicar win-x64 con `scripts\publish.ps1`, instalar Inno Setup 6, ejecutar `scripts\build-installer.ps1` y probar instalación/desinstalación en VM limpia (verificar que los datos de `%LOCALAPPDATA%\RutaTI30` sobreviven a la desinstalación).
