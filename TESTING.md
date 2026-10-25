# TESTING — Ruta TI 30

## 1. Pruebas ejecutadas en este entorno (evidencia real)

| Comando | Fecha | Resultado |
|---|---|---|
| `dotnet --info` / `dotnet --list-sdks` | 2026-10-09 | **Fallido**: `bash: dotnet: command not found` (exit 127) |
| `uname` + `command -v dotnet\|pwsh\|iscc\|wine` (cierre definitivo) | 2026-10-09 | Linux x86_64; las 4 herramientas **ausentes**: validación de Windows imposible en este entorno |
| Correspondencia rutas scripts ↔ repositorio (`RutaTI30.sln`, csproj App, `.iss`, salida `artifacts\win-x64\RutaTI30.exe`) | 2026-10-09 | **Verificado** (lectura directa de los 4 scripts) |
| `dotnet build` | — | **No ejecutado**: SDK ausente |
| `dotnet test` | — | **No ejecutado**: SDK ausente |
| Revisión estática de archivos y referencias entre proyectos (lectura de `.csproj`, `.sln`, XAML, C#) | 2026-10-09 | Ejecutada con herramientas del entorno; sin hallazgos tras correcciones |
| Revisión de seguridad: SQL con parámetros (48 sentencias / 229 refs `@`), sin secretos incrustados, sin `HttpClient`/`Process.Start`, catches vacíos justificados (2, documentados) | python3 + grep | **Aprobado** |
| Verificación tras enriquecimiento de contenido (30 días) y `JobRules`: balance léxico 65/65 `.cs`, 120 preguntas con ids únicos y formato estándar, VM usa `JobRules.Validate` en alta y edición | python3 | **Aprobado** |
| Fase de excelencia (segunda sesión) — verificación estática de cierre (2026-10-10): balance léxico **67/67** `.cs` (incluye `TicketPracticeScenarios`, `InterviewQuestions` actualizado, `CareerViewModel` con borrado), XML **15/15** (8 XAML + 5 csproj/props/props/manifest), **228 `{Binding}` resueltos de las vistas** (incluye los campos `record` `ApplicationItemVm`, `HistoryVm`, `TicketItemVm`, `AttemptVm`, `ConceptVm`, `OptionLabel` y los nuevos lectores de `CareerViewModel`/`TicketsViewModel`), contenido **30 días / 120 preguntas / 21 de entrevista / 9 casos** con ids únicos, sin `HttpClient`/`Process.Start`/red en código fuente, todos los enlaces locales de `.md` resueltos | python3 + grep | **Aprobado** |
| Fase de excelencia — verificación estática final (2026-10-09): balance léxico **67/67** `.cs`, XML **15/15** (8 XAML + 5 csproj + props + manifest), bindings **228/228** resueltos (incluidos los campos de los `record` de los ViewModels), contenido **30 días / 120 preguntas / 21 de entrevista / 9 casos de práctica** con ids únicos, sin llamadas de red, links locales de `.md` íntegros | python3 + grep | **Aprobado** |
| Sesión educativa (2026-10-10) — `python3 scripts/verify/static-regression.py`: XML **16/16**, balance léxico **75/75** `.cs`, **216 `{Binding}` resueltos** (incluye Glosario y Rutas), sin llamadas de red, sin interpolación de SQL | terminal | **Aprobado** |
| Sesión educativa (2026-10-10) — `python3 scripts/verify/edu-content-audit.py`: **30 lecciones / 120 preguntas / 34 entrevistas / 16 escenarios / 10 laboratorios**, ids únicos, índices de respuesta correctos, campos obligatorios completos | terminal | **Aprobado** |
| Ampliación de escenarios (9 → 16), entrevistas (21 → 34 en 17 áreas), repaso espaciado, glosario (**49 términos**, 5 categorías) y rutas (5): revisión de enlaces de contenido contra catálogos reales (días, ids `prac-*`, áreas del banco) | python3 | **Aprobado** |
| Recuperación y coherencia de integración (2026-10-11): `static-regression` (XML 16/16, léxico 81/81 `.cs`, bindings 216/216), `edu-content-audit` (59 términos de glosario con categorías y cuasi-duplicados revisados), `d09-wire-foundations-verification`, `git diff --check` | terminal, exit codes reales | **PASSED** (los cuatro, exit 0) |
| Defecto P0 de compilación detectado y corregido el 2026-10-11: `BackupService` referencia `NullDbStateChanged.Instance` que **no existía** (no compilaba); se creó la clase nula en `Infrastructure` y se añadió la notificación `_dbStateChanged.Raise()` al final de una restauración exitosa | lectura del código + grep | **Corregido** (verificación de compilación real pendiente en Windows) |
| Cableado F-05 completado el 2026-10-11: `MainViewModel` ahora se suscribe a `IDbStateChanged.DataChanged` y refresca los módulos con estado cargado al restaurar una copia; antes el evento existía pero **nadie escuchaba** | lectura del código | **Corregido** |
| Pruebas xUnit añadidas el 2026-10-11 (no ejecutadas todavía: el entorno carece de SDK .NET): `BackupServiceTests.Constructor_SinNotificador_ConsienteDependencia`, `Restore_Exitoso_NotificaCambioDeBaseDeDatos`, `Restore_Fallido_NoNotifica` | edición de `tests/RutaTI30.Tests/Infrastructure/BackupServiceTests.cs` | **Escritas** (ejecución pendiente en Windows) |
| Misión 2.1 — auditoría de hallazgos nuevos (2026-10-12): el proyecto de pruebas hace referencia a `RutaTI30.App` (`RutaTI30.Tests.csproj` con `net10.0-windows`/`UseWPF`); `DialogService` no es sellada (métodos virtuales para `DialogServiceStub` en `StudyTimerTickIntegrationTests.cs`) y las APIs usadas desde pruebas existen (`StudyTimerService.Tick/ElapsedSeconds/Load/Reset`, `AsyncRelayCommand.ExecuteTask`, `AppDb.DatabasePath`, `IDbStateChanged`) | lectura directa de archivos | **Verificado** |
| Misión 2.1 — Defecto P0 confirmado y corregido (2026-10-12) en `tests/RutaTI30.Tests/Application/StudyTimerTickIntegrationTests.cs`: la prueba `Start_Pausa_Reanudacion_YFinalizacion_PersistenSoloTiempoVerificado` carecía de **llave de cierre** (la clase se superponía al `[Fact]` siguiente; balance léxico 80/81 → FAIL). Se añadió `}` | `python3 scripts/verify/static-regression.py` | **Corregido**: static-regression 81/81 `.cs` PASSED (exit 0). Compilación real pendiente en Windows |
| Misión 2.1 — verificación al cierre (2026-10-12): `static-regression` PASSED (XML 16/16, léxico 81/81, bindings 216/216, sin red, SQL parametrizado), `edu-content-audit` PASSED (30/120/34/16/10/59), `d09-wire-foundations-verification` PASSED, `git diff --check` limpio, stash 961d2fb intacto | terminal, exit codes reales | **PASSED** (los cinco) |
| Misión 2.2 (2026-10-12) — corrección documental: se detectaron **nombres de API inexistentes** en dos filas duplicadas de este archivo (`EvaluateAttemptAsync`, `PrepareDescription`: verificados con grep, 0 coincidencias en src/ y tests/). Se deduplicaron las filas de Misión 2.1 y se reemplazaron los nombres inventados por los reales: `EvaluationService.Evaluate(lesson, selectedIndexes)` y `AppDb.DatabasePath` | grep + lectura directa | **Corregido** |
| Misión 2.2 (2026-10-12) — auditoría de scripts y compatibilidad: `RutaTI30.sln` incluye los 5 proyectos (dotnet build/test sobre la solución compilan y prueban todo); `build.ps1`/`test.ps1`/`publish.ps1`/`build-installer.ps1` propagan `$LASTEXITCODE` correctamente, hacen `Set-Location` a la raíz del repo y no eliminan datos de usuario; TFMs consistentes (Domain/Application/Infrastructure `net10.0`; App/Tests `net10.0-windows` + `UseWPF`); suite real = **23 archivos de prueba** (la cifra «18 archivos» de VERIFY 1.2 estaba desactualizada) | lectura directa de scripts/.sln/csproj + find | **Verificado** |
| Misión 5.0 (2026-10-13) — validación final, fases ejecutables: **Fase 1 (diagnóstico)**: rama `main`, HEAD `9afd8ad`, sin commits/push; 33 archivos modificados + 22 sin seguimiento; objeto `961d2fb` intacto (tipo `commit`); sin artefactos previos (`find *.exe|*.msi|*setup*` → 0). **Fases 2–5**: comandos Windows nunca ejecutados — evidencia directa: `command -v` de `dotnet/pwsh/powershell/iscc/wine/msbuild/mono` → las 7 AUSENTES; `dotnet --version` → `bash: dotnet: command not found`; `pwsh -File scripts/test.ps1` → `pwsh: command not found` (ninguna fase simulada). **Fase 6 (cierre)**: `static-regression.py` PASSED (XML 16/16, léxico 81/81, bindings 223/223, sin red, SQL parametrizado, exit 0); `edu-content-audit.py` PASSED (30/120/34/16/10/59, exit 0); `d09-wire-foundations-verification.py` exit 0; `git diff --check` limpio (exit 0); diff global: 33 archivos, +1051/−69 | terminal, exit codes reales | Fases 1 y 6: **APROBADAS (estáticas)**; Fases 2–5: **BLOQUEADAS por entorno** |
| Misión 6.0 (2026-10-13): kit visual ampliado sin tocar lógica: entrada de mensajes 180 ms (`MsgBase` ya heredado por `MsgInfo/Success/Warning/Error`), press 120 ms sin storyboards conflictivos, insignia de estado semántica en vista con DataTriggers (3 bindings nuevos de texto), `MsgInfo` en bienvenida/evaluación/resultado de laboratorio, lectura 760 px en entrevistas. Cero cambios en ViewModels mencionables/enviados: los únicos `.cs` tocados en 3.0–6.0 siguen siendo `HomeViewModel` (+ acceso `OpenModuleCommand` read-only) | terminal con exit codes reales | **APROBADA (estática)**: static-regression PASSED (XML 16/16, léxico 81/81, bindings 226/226), auditoría de recursos 19 claves/0 faltantes, `git diff --check` limpio; render WPF pendiente en Windows |
| Misiones 7.0–9.0 (2026-10-13): **M7** — auditoría con strings reales confirmados (`StateLabel` → «No iniciado/En curso/Completado/Dominado» en `LearnViewModel.cs:399-405`; prioridad → «P1 · Crítica/P2 · Alta/P3 · Media/P4 · Baja» en `TicketsViewModel.cs:528-531` y `TicketRules.CalculatePriority`): 2 insignias restantes con fondo de acción corregidas a semántica nominal con DataTriggers (ModuleTemplates/WorkTemplates). **M8** — re-verificación: static-regression PASSED (XML 16/16, léxico 81/81, bindings 232/232, sin red, SQL parametrizado, exit 0), edu-content-audit PASSED (exit 0), d09 PASSED (exit 0), `git diff --check` limpio (exit 0). **M9** — 0 artefactos (`*.exe`/`*.msi`/`*setup*`); nada simulado | terminal con exit codes reales | M7/M8: **APROBADAS (estática)**; compilación/pruebas/visual/publicación: **BLOQUEADAS** (Linux sin .NET 10/PowerShell/ISCC; casillas VERIFY §1–§4 «Pendiente») |
| Auditoría integral final (2026-10-13): prueba funcional estática de la UI sin ediciones: 42/42 botones de vistas con `Command` (análisis multilínea Python; la heurística por línea era fallida), 0 `ListBox` sin `ItemsSource` (8 falsos positivos = subetiquetas ItemTemplate/ItemContainerStyle), 9 módulos reales en `NavItems`; matriz de las 14 capacidades objetivo y brechas P0–P3 registradas en PROGRESS.md; hallazgo principal: **Diagnóstico inicial y Mapa de competencias formal no implementados** (PROPUESTOS: requieren C# nuevo y sin compilador no se escribe a ciegas) | grep + script Python + lectura directa | **INSPECCIONADO y VALIDADO ESTÁTICAMENTE**; validación visual/bundle Windows siguen pendientes (VERIFY §1–§2) |
| Auditoría lingüística y de codificación (2026-10-13): barrido de caracteres por UTF-8/Unicode (patrones `Ã/Â/â€/�` y más con control fuera de los signos esperados) → **0 hallazgos**; marcadores `TODO/FIXME/TBD/XXX:` → 1 falsa coincidencia en contenido educativo («retirar TODO al salir», `LessonsBloque4.cs`) **corregido** a «retirar todo» (P3; el texto debía leerse naturalmente, sin ambiguanza con la marca de código); palabras duplicadas dentro de cadenas C# → 1 hallazgo real: «entre sí sí se comunican» (`LabScenarios.cs:29`) es gramatical, pero ambiguo para principiantes → **redactado** «sí se comunican entre sí» (mismo significado, contexto de laboratorio, sin tocar opciones/respuestas); barrido de tildes faltantes en palabras frecuentes y dobles espacios en `Text/Content/ToolTip` → **0 hallazgos** | grep + scripts Python + re-ejecución | **APROBADAS (estática)**: static-regression PASSED (exit 0), edu-content-audit PASSED (exit 0, conteos intactos 30/120/34/16/10/59), diff-check limpio; validación en pantalla pendiente (VERIFY §2) |

Estado honesto: **las pruebas están escritas pero ninguna ha sido ejecutada todavía** (falta el SDK .NET). Se encontraron y corrigieron errores de compilación mediante revisión estática de referencias, bindings de XAML y XPath de plantillas; la compilación y la ejecución de `dotnet test` siguen pendientes en Windows.

## 2. Procedimiento de validación (Windows 10/11 x64 con .NET 10 SDK)

Pasos completos, con resultados esperados y registro de resultados: **VERIFY.md**. Resumen ejecutable:

```powershell
# 0) Requisitos: Windows 10/11 x64 y .NET 10 SDK (winget install Microsoft.DotNet.SDK.10
#    o descarga de https://dotnet.microsoft.com/download). No requiere conexión para USAR la app,
#    solo para instalar el SDK y restaurar paquetes.

# 1) Comprobar el SDK (debe listar una versión 10.x):
dotnet --list-sdks

# 2) Abrir PowerShell EN LA RAÍZ del repositorio (donde está RutaTI30.sln):
cd <ruta-del-repo>

# 3) Restaurar paquetes (confirma las versiones candidatas de DECISIONS.md D8):
dotnet restore RutaTI30.sln

# 4) Compilar Debug y Release (scripts/build.ps1 hace restore+build de ambas y propaga errores):
powershell -ExecutionPolicy Bypass -File scripts\build.ps1

# 5) Ejecutar TODA la suite xUnit (23 archivos; falla con exit != 0 si hay errores):
powershell -ExecutionPolicy Bypass -File scripts\test.ps1

# 6) Filtrar errores: dotnet test imprime «Failed!» por prueba; para repetir solo una:
dotnet test tests/RutaTI30.Tests/RutaTI30.Tests.csproj -c Release --filter "FullyQualifiedName~NombreDeLaPrueba"

# 7) Iniciar la aplicación WPF:
dotnet run --project src\RutaTI30.App
```

Si una prueba falla: registrar nombre de la prueba, mensaje completo de aserción, versión de SDK
(`dotnet --info`) y versión de Windows; corregir el código (no la aserción) y reejecutar.
No marcar ninguna casilla de VERIFY.md sin evidencia real.

## 3. Suite escrita (no ejecutada)

| Archivo | Cubre |
|---|---|
| `Domain/TicketRulesTests.cs` | Cálculo de prioridad, transiciones de estado válidas/inválidas |
| `Domain/JobRulesTests.cs` | Validación de postulaciones: campos obligatorios, enlace http(s), coherencia de fechas (descubrimiento ≤ postulación ≤ entrevista) |
| `Application/EvaluationServiceTests.cs` | Puntaje, umbral 80 %, temas débiles, preguntas sin responder |
| `Application/ProgressServiceTests.cs` | Estadísticas calculadas de datos reales, no fabricados |
| `Application/ScheduleServiceTests.cs` | Bloques por hora, validación de horario inválido |
| `Application/StudyTimerServiceTests.cs` | Inicio/pausa/reanudación/finalizar, no contar pausas, recuperación tras cierre sin inflar tiempo |
| `Application/DashboardServiceTests.cs` | Cálculos del panel con datos vacíos y con datos |
| `Infrastructure/MigrationTests.cs` | Inicialización, `user_version`, esquema, consultas parametrizadas |
| `Infrastructure/LearningStoreTests.cs` | Persistencia de progreso e intentos tras "reabrir" la BD |
| `Infrastructure/TicketStoreTests.cs` | Alta de ticket + historial transaccional, filtros y búsqueda |
| `Infrastructure/JobStoreTests.cs` | CRUD de postulaciones y seguimientos; **borrado en cascada** (elimina postulación y seguimientos, no afecta a otras, id inexistente sin error) |
| `Infrastructure/BackupServiceTests.cs` | Copia válida, restauración validada, rechazo de copia corrupta |
| `Application/LabScoringTests.cs` | Ponderación de decisiones del laboratorio |
| `Application/PracticeScenarioTests.cs` | 16 casos de práctica del simulador: ids únicos, campos didácticos completos, cobertura DNS/DHCP/IP, temas nuevos (VPN, correo, post-actualización, solicitud de permisos, incidente multiusuario, contraseña, hardware), niveles de impacto/urgencia válidos, sin acciones inseguras |
| `Application/ContentTests.cs` | Integridad del contenido: 30 días únicos, estructura pedagógica completa, IDs de preguntas únicos, campos obligatorios del banco de entrevistas, cobertura de las 14 áreas requeridas (≥ 30 preguntas), tipos de pregunta (técnica/conductual/diagnóstico) |
| `Application/ReviewServiceTests.cs` | Repaso espaciado: fallo programa repaso (1 día), intervalos crecientes con fallos repetidos, alargado con aciertos, temas no fallados fuera de la cola, detalles ilegibles sin romper el cálculo, persistencia tras reiniciar |
| `Application/GlossaryTests.cs` | Glosario: ≥ 30 entradas con ids/términos únicos, campos obligatorios completos, enlaces a lecciones (1–30) y conceptos existentes, búsqueda insensible a mayúsculas y acentos, filtro por categoría, 5 categorías cubiertas |
| `Application/SpecializationRouteTests.cs` | Rutas: 5 itinerarios con ids únicos, campos completos, lecciones (1–30) existentes, ids `prac-*` existentes, áreas presentes en el banco de entrevistas |
| `Application/AttemptDetailsTests.cs` | Detalle JSON de intentos: respuestas correctas/incorrectas/sin responder distinguidas, intentos antiguos sin «correct» no se marcan como acierto, JSON malformado no rompe el cálculo |
| `Application/BackupServiceIntegrationTests.cs` | Integración de restauración: notificación `IDbStateChanged` tras restaurar con éxito y observador que recarga datos reales desde la base restaurada |
| `Application/DependencyCompositionTests.cs` | Inicialización de dependencias: el contenedor resuelve catálogos y ViewModels (Glosario, Rutas) con los registros reales; contratos de catálogos con datos reales |
| `Application/ProgressWeakTopicsIntegrationTests.cs` | Temas débiles/demostrados desde datos reales: fallo explícito, acierto explícito, sin corrección registrada (no se inventa), detalles vacíos/malformados, último intento manda |
| `Application/StudyTimerTickIntegrationTests.cs` | Temporizador integrado con `HomeViewModel`: ticks de UI, pausa no acumula, finalización persiste tiempo efectivo, cierre inesperado → recuperación solo con tiempo verificado, reset bloqueado en curso |

## 3.bis Procedimiento de validación en Windows

El checklist detallado (comandos, resultados esperados y casillas) vive en **VERIFY.md**. Todas sus casillas están **pendientes** hasta ejecutarse en Windows.

## 4. Plan de verificación manual (pendiente, Windows)

1. Compilación Debug y Release (`scripts/build.ps1`).
2. Inicio de la aplicación: aparece la ventana y el panel de inicio.
3. Navegación por las 9 secciones (incluye Glosario y Rutas) y con teclado (Tab/Enter).
4. Creación de la base de datos en `%LOCALAPPDATA%/RutaTI30/`.
5. Persistencia: marcar una lección, cerrar y reabrir → el estado permanece.
6. Temporizador: iniciar, pausar, reanudar, finalizar; cerrar en curso → recuperación sin tiempo inflado.
7. Tickets: crear, cambiar estado inválido (debe rechazarse), resolver, historial visible.
8. Estadísticas: valores coinciden con datos reales (sin cifras inventadas).
9. Copia de seguridad y restauración con archivo inválido (debe rechazarse).
10. Funcionamiento sin conexión: deshabilitar red y repetir flujos principales.
11. Publicación win-x64 y primer inicio del artefacto publicado.
