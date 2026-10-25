# AUDIT_TRACEABILITY — Ruta TI 30

> **Snapshot histórico (2026-10-09)**: la matriz refleja el estado de la auditoría. Requisitos nuevos (R-24 glosario, R-25 rutas, R-26 repaso espaciado, R-27 entrevistas ampliadas) y la navegación de 9 secciones están en REQUIREMENTS.md.

Matriz de trazabilidad entre requisito, implementación, pruebas y evidencia real hasta el entorno actual.

## Leyenda de estados

- **Implementado, sin validar**: existe el código, pero no se ejecutó confirmación en tiempo de ejecución.
- **Validado estáticamente**: revisado mediante lectura y análisis de archivos sin ejecutar la aplicación.
- **Compilado**: existe evidencia de compilación.
- **Prueba automatizada aprobada**: suite ejecutada y aprobada.
- **Validado funcionalmente**: se recorrió en la aplicación.
- **Bloqueado por entorno**: no se pudo ejecutar aquí.
- **Fallido**: se ejecutó y no pasó.
- **Pendiente**: no está listo o no se ejecutó.

## Matriz

| ID requisito | Qué requiere | Implementación real | Prueba real | Evidencia real | Estado |
|---|---|---|---|---|---|
| R1 | Dominio sin dependencias de UI/datos | `RutaTI30.Domain` sin referencias a WPF/SQLite/App | Revisión de archivos | Lectura de `.csproj` y código | Validado estáticamente |
| R2 | Servicios de aplicación independientes de UI | Servicios en `RutaTI30.Application`, almacenes inyectados vía interfaces | Revisión de archivos | Código leído | Validado estáticamente |
| R3 | Persistencia SQLite parametrizada | `AppDb`, almacenes, `DbFormat`, consultas con parámetros | Revisión de archivos + tests existentes | Código leído; tests no ejecutados | Validado estáticamente; pruebas bloqueadas por entorno |
| R4 | Migraciones versionadas en transacción | `Migrations`, `AppDb.Initialize` | Revisión de archivos + `MigrationTests` | Código leído; tests no ejecutados | Validado estáticamente; pruebas bloqueadas por entorno |
| R5 | Backup y restauración controlados | `BackupService` con validación, respaldo previo y restauración | `BackupServiceTests` diseñados para casos inválidos/corruptos | Código leído; tests no ejecutados | Validado estáticamente; pruebas bloqueadas por entorno |
| R6 | 30 días de contenido con estructura pedagógica | `LessonCatalog` + bloques 1..5 | `ContentTests` | Código leído; tests no ejecutados | Validado estáticamente; pruebas bloqueadas por entorno |
| R7 | Evaluaciones con umbral y explicación | `EvaluationService`, `AttemptDetails` | `EvaluationServiceTests` | Código leído; tests no ejecutados | Validado estáticamente; pruebas bloqueadas por entorno |
| R8 | Planificador editable | `ScheduleService`, preferencias JSON | `ScheduleServiceTests` | Código leído; tests no ejecutados | Validado estáticamente; pruebas bloqueadas por entorno |
| R9 | Temporizador con recuperación tras cierre | `StudyTimerService`, `timer_state`, `SessionStore` | `StudyTimerServiceTests` | Código leído; tests no ejecutados | Validado estáticamente; pruebas bloqueadas por entorno |
| R10 | Laboratorio simulado sin ejecutar procesos | `Lab*`, `LabScoring` | `LabScoringTests` | Código leído; tests no ejecutados | Validado estáticamente; pruebas bloqueadas por entorno |
| R11 | Tickets: CRUD, prioridad, transiciones, historial | `TicketsViewModel`, `TicketStore`, `TicketRules` | `TicketRulesTests`, `TicketStoreTests` | Código leído; tests no ejecutados | Validado estáticamente; pruebas bloqueadas por entorno |
| R12 | Casos de práctica de Help Desk con guía | `TicketPracticeScenarios`, carga en formulario, `PracticeGuide` | `PracticeScenarioTests` | Código leído; tests no ejecutados | Validado estáticamente; pruebas bloqueadas por entorno |
| R13 | Banco de entrevistas con autoevaluación y guía de fortalezas | `InterviewBank`, `CareerViewModel`, `RebuildInterviewGuide` | `ContentTests` y `CareerViewModel` en vistas | Código leído; tests no ejecutados | Validado estáticamente; pruebas bloqueadas por entorno |
| R14 | Postulaciones con validación y borrado en cascada | `JobRules`, `JobStore.DeleteApplication`, comando con confirmación | `JobRulesTests`, `JobStoreTests` (incluye borrado en cascada) | Código leído; tests no ejecutados | Validado estáticamente; pruebas bloqueadas por entorno |
| R15 | Navegación de 7 secciones y datos compartidos | `MainViewModel`, plantillas por ViewModel | Inspección de App.xaml y XAML | Revisión de archivos | Validado estáticamente |
| R16 | Tema oscuro y claro | `Dark.xaml`, `Light.xaml`, `ThemeService` | Revisión de archivos | Revisión de archivos | Validado estáticamente |
| R17 | Funcionamiento sin Internet | Diseño sin red, contenido embebido | Revisión de arquitectura | Revisión de archivos | Validado estáticamente |
| R18 | Compilación Debug/Release | Scripts `build.ps1` | `build.ps1` | Script existe; no se ejecutó | Bloqueado por entorno |
| R19 | Pruebas automatizadas aprobadas | Suite xUnit existente | `scripts/test.ps1` | 16 archivos, ~103 métodos diseñados; no se ejecutó | Bloqueado por entorno |
| R20 | Publicación win-x64 | `scripts/publish.ps1` | Script existe | Script existe; no se ejecutó | Bloqueado por entorno |
| R21 | Instalador por usuario sin admin | `installer/rutati30.iss` + `build-installer.ps1` | Script existe | Script existe; no se compiló ni probó | Bloqueado por entorno |
| R22 | Conservación de datos en desinstalación | Diseño de instalador y BACKUP_RESTORE | Revisión de INSTALLATION.md y .iss | Revisión de archivos | Validado estáticamente; instalación bloqueada por entorno |

---

*Matriz de trazabilidad generada como entregable de la auditoría integral.*
