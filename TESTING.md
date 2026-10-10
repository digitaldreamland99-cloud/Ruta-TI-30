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

Estado honesto: **las pruebas están escritas pero ninguna ha sido ejecutada todavía** (falta el SDK .NET). Se encontraron y corrigieron errores de compilación mediante revisión estática de referencias, bindings de XAML y XPath de plantillas; la compilación y la ejecución de `dotnet test` siguen pendientes en Windows.

## 2. Cómo ejecutar las pruebas (Windows con .NET 10 SDK)

```powershell
# Compilar todo (Debug)
powershell -File scripts/build.ps1

# Ejecutar pruebas
powershell -File scripts/test.ps1
```

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
| `Application/PracticeScenarioTests.cs` | 9 casos de práctica del simulador: ids únicos, campos didácticos completos, cobertura DNS/DHCP/IP, niveles de impacto/urgencia válidos, datos ficticios identificados |
| `Application/ContentTests.cs` | Integridad del contenido: 30 días únicos, estructura pedagógica completa, IDs de preguntas únicos, campos obligatorios del banco de entrevistas, IDs de casos de práctica únicos (9), cobertura DNS/DHCP/IP, niveles de impacto/urgencia válidos |

## 3.bis Procedimiento de validación en Windows

El checklist detallado (comandos, resultados esperados y casillas) vive en **VERIFY.md**. Todas sus casillas están **pendientes** hasta ejecutarse en Windows.

## 4. Plan de verificación manual (pendiente, Windows)

1. Compilación Debug y Release (`scripts/build.ps1`).
2. Inicio de la aplicación: aparece la ventana y el panel de inicio.
3. Navegación por las 7 secciones y con teclado (Tab/Enter).
4. Creación de la base de datos en `%LOCALAPPDATA%/RutaTI30/`.
5. Persistencia: marcar una lección, cerrar y reabrir → el estado permanece.
6. Temporizador: iniciar, pausar, reanudar, finalizar; cerrar en curso → recuperación sin tiempo inflado.
7. Tickets: crear, cambiar estado inválido (debe rechazarse), resolver, historial visible.
8. Estadísticas: valores coinciden con datos reales (sin cifras inventadas).
9. Copia de seguridad y restauración con archivo inválido (debe rechazarse).
10. Funcionamiento sin conexión: deshabilitar red y repetir flujos principales.
11. Publicación win-x64 y primer inicio del artefacto publicado.
