# REQUIREMENTS — Ruta TI 30

Estados: `P` Pendiente · `D` En desarrollo · `SV` Implementado sin verificar · `V` Verificado · `B` Bloqueado (entorno)

| ID | Requisito | Módulo | Estado | Implementación | Prueba | Resultado |
|----|-----------|--------|--------|----------------|--------|-----------|
| R-01 | Aplicación WPF ejecutable en Windows x64 | App | SV | `src/RutaTI30.App` | `scripts/build.ps1` + ejecución manual | No ejecutada (sin SDK) |
| R-02 | Navegación lateral de 9 módulos (7 originales + Glosario y Rutas) | App | SV | `MainViewModel`, plantillas en `App.xaml` | Manual: recorrer las 9 secciones | No ejecutada |
| R-03 | Panel de inicio con datos reales | Inicio | SV | `DashboardService`, `HomeViewModel` | `DashboardServiceTests` (escritas) | No ejecutadas |
| R-04 | Base de datos SQLite inicializada y versionada | Datos | SV | `AppDb.Initialize`, migración M001 | `MigrationTests` (escritas) | No ejecutadas |
| R-05 | Persistencia tras reiniciar | Datos | SV | Almacenes SQLite | `LearningStoreTests` (escritas) | No ejecutadas |
| R-06 | Ruta de 30 días con contenido real | Formación | SV | `LessonsBloque1..5.cs` | Revisión de contenido + `ContentTests.cs` | No ejecutadas |
| R-07 | Estados de dominio (no iniciado/en curso/completado/dominado) | Formación | SV | `LessonState`, `LearningStore` | `ProgressServiceTests` | No ejecutadas |
| R-08 | Evaluaciones con umbral y soluciones | Formación | SV | `EvaluationService` | `EvaluationServiceTests` | No ejecutadas |
| R-09 | Temporizador con pausa/reanudación/recuperación | Temporizador | SV | `StudyTimerService` | `StudyTimerServiceTests` | No ejecutadas |
| R-10 | Horario editable | Planificador | SV | `ScheduleService` | `ScheduleServiceTests` | No ejecutadas |
| R-11 | Laboratorio simulado con decisiones que afectan resultado | Laboratorio | SV | `LabCatalog`, `LabViewModel` | `LabScoringTests` | No ejecutadas |
| R-12 | Tickets: CRUD, estados válidos, historial | Help Desk | SV | `TicketStore`, `TicketRules` | `TicketRulesTests`, `TicketStoreTests` | No ejecutadas |
| R-13 | Prioridad calculada por impacto×urgencia | Help Desk | SV | `TicketRules.CalculatePriority` | `TicketRulesTests` | No ejecutadas |
| R-14 | Banco de entrevistas con respuestas, autoevaluación y guía de fortalezas | Preparación | SV | `InterviewBank`, `JobStore`, `CareerViewModel` | `ContentTests.cs` | No ejecutadas |
| R-14b | Casos de práctica para el simulador de Help Desk (16 casos) | Help Desk | SV | `TicketPracticeScenarios`, `TicketsViewModel`, UI | `PracticeScenarioTests.cs` | No ejecutadas |
| R-24 | Glosario local con búsqueda sin conexión y filtro por categoría | Glosario | SV | `Glossary`/`GlossaryCatalog`, `GlossaryViewModel`, `GlossaryTemplates.xaml` | `GlossaryTests.cs` | No ejecutadas |
| R-25 | Rutas de especialización posteriores al día 30, enlazadas con contenido existente | Rutas | SV | `SpecializationRoutes`, `RoutesViewModel`, `RoutesTemplates.xaml` | `SpecializationRouteTests.cs` | No ejecutadas |
| R-26 | Repaso inteligente con intervalos crecientes basado en intentos reales | Progreso | SV | `ReviewService`, sección en `ProgressViewModel` + UI | `ReviewServiceTests.cs` | No ejecutadas |
| R-27 | Banco de entrevistas ampliado (34 preguntas, 17 áreas) | Preparación | SV | `InterviewQuestions.cs` | `ContentTests.cs` | No ejecutadas |
| R-15 | Registro de postulaciones CRUD | Preparación | SV | `CareerViewModel`, `JobStore` | `JobStoreTests` | No ejecutadas |
| R-15b | Eliminación de postulaciones con confirmación y borrado en cascada | Preparación | SV | `IJobStore.DeleteApplication`, `CareerViewModel.DeleteApplicationCommand`, UI con botón + tooltip | `JobStoreTests` (3 pruebas nuevas) | No ejecutadas |
| R-16 | Estadísticas calculadas de datos persistidos | Progreso | SV | `ProgressService` | `ProgressServiceTests` | No ejecutadas |
| R-17 | Copia de seguridad y restauración validada | Datos | SV | `BackupService` | `BackupServiceTests` | No ejecutadas |
| R-18 | Tema oscuro por defecto y tema claro | UI | SV | `Themes/Dark.xaml`, `Themes/Light.xaml`, `ThemeService` | Manual en Windows | No ejecutada |
| R-19 | Funcionamiento sin Internet | Global | SV | Sin red en el código | Revisión de dependencias + prueba manual | No ejecutada |
| R-20 | Compilación Debug y Release | Build | B | `scripts/build.ps1` | Ejecutar en Windows con SDK | Bloqueado por entorno |
| R-21 | Pruebas automatizadas aprobadas | Calidad | B | `tests/RutaTI30.Tests` | `scripts/test.ps1` | Bloqueado por entorno |
| R-22 | Publicación win-x64 | Publicación | B | `scripts/publish.ps1` | Ejecutar en Windows | Bloqueado por entorno |
| R-23 | Instalador | Publicación | SV | `installer/rutati30.iss`, `scripts/build-installer.ps1` | Compilación con ISCC.exe + prueba en VM limpia | Configurado (Inno Setup 6, per-user, conserva datos); pendiente de compilar/probar en Windows |

> Todo requisito en estado `SV` o `B` **no** puede darse por aprobado: su prueba real está pendiente de un entorno Windows con .NET 10 SDK.
