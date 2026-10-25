# CHANGELOG — Ruta TI 30

Formato: [Keep a Changelog] en español. Versión inicial **0.1.0 (en desarrollo)**.

## [Sin publicar]

### Añadido (sesión educativa, 2026-10-10)
- **Glosario local** (`Content/Glossary.cs`, `GlossaryViewModel`, `GlossaryTemplates.xaml`): 49 términos en 5 categorías (Computación y hardware, Windows y diagnóstico, Redes, Service Desk, Seguridad e IAM) con definición sencilla, explicación técnica, ejemplo laboral, error frecuente y enlaces a lecciones existentes; búsqueda insensible a mayúsculas y acentos, filtro por categoría y navegación sin conexión.
- **Rutas posteriores al día 30** (`Content/SpecializationRoutes.cs`, `RoutesViewModel`, `RoutesTemplates.xaml`): 5 itinerarios opcionales (Service Desk L1, soporte de aplicaciones, infraestructura y redes, Windows y Microsoft 365, IAM) con prerrequisitos, competencias, lecciones base (días 1–30), prácticas asociadas (ids `prac-*`) y áreas del banco de entrevistas; declarados como opcionales, no como requisito de un primer empleo L1.
- **Simulador de Help Desk ampliado**: 9 → **16 casos de práctica** (añadidos VPN, correo que no sincroniza, fallo tras actualización, solicitud de permisos, incidente multiusuario, contraseña no válida en aplicación y equipo que no enciende), con las mismas secciones didácticas y datos ficticios identificados. *(Restaurado y de nuevo en el árbol el 2026-10-11; ver PROGRESS.md).*
- **Banco de entrevistas ampliado**: 21 → **34 preguntas en 17 áreas**, con competencia evaluada, respuesta modelo, explicación, errores frecuentes y seguimientos. *(Restaurado y de nuevo en el árbol el 2026-10-11).*
- **Glosario ampliado 49 → 59 términos** (2026-10-11): phishing, malware, ransomware, copia de seguridad, antivirus/EDR, actualizaciones, Windows Update y recuperación, CMD y PowerShell, CV orientado a soporte y laboratorio doméstico; auditoría de glosario integrada en `scripts/verify/edu-content-audit.py`.
- **Repaso espaciado** (`Services/ReviewService.cs`, sección en Mi progreso): cola de repaso derivada solo de intentos reales; conceptos fallados con intervalos crecientes (1, 2, 4, 8, 14 días), alargados con aciertos consecutivos; visible en la interfaz de Progreso.
- **Pruebas nuevas**: `GlossaryTests.cs`, `SpecializationRouteTests.cs`, `ReviewServiceTests.cs`; `PracticeScenarioTests` y `ContentTests` ampliadas a 16 escenarios y 34 preguntas.
- **Verificadores estáticos**: `scripts/verify/static-regression.py` (XML, balance léxico, resolución de bindings, sin red, SQL parametrizado) y `scripts/verify/edu-content-audit.py` (conteos reales de contenido e ids únicos); ambos aprobados.
- Navegación lateral: 7 → **9 secciones** (Glosario y Rutas integradas en la navegación existente).

- **Corrección de inicio (P0)**: registro faltante de `IGlossaryCatalog` en el contenedor DI de `App.xaml.cs`; sin esta línea el módulo «Glosario» no se activaba en tiempo de ejecución por falta de dependencia inyectable.

### Corregido (2026-10-11, segunda sesión)
- **P0 de compilación**: referencia a `NullDbStateChanged.Instance` sin clase definida en `Infrastructure/BackupService.cs` (residuo del conflicto stash/árbol). Creada la implementación nula de `IDbStateChanged`.
- **P1 — vistas desactualizadas tras restaurar (F-05)**: la notificación de cambio de base de datos existía en DI pero ningún suscriptor la escuchaba. Ahora `BackupService.Restore()` levanta el evento solo si la restauración fue exitosa, y `MainViewModel` refresca los módulos con datos cargados al recibirla.
- **Contenido**: dos términos del glosario (Windows Update y recuperación; CMD y PowerShell) tenían la categoría «Windows» en vez de «Windows y diagnóstico»; corregido y ahora detectado automáticamente por la auditoría.
- **Auditoría**: `edu-content-audit.py` ahora valida categorías del glosario y detecta cuasi-duplicados por definición inicial idéntica.

## [0.1.0] — 2026-10-09

### Añadido
- Solución `RutaTI30.sln` con proyectos Domain, Application, Infrastructure, App (WPF) y Tests.
- Esquema SQLite inicial versionado (M001) con migraciones transaccionales y consultas parametrizadas.
- Servicios de negocio: progreso, evaluaciones (umbral 80 %), planificador, temporizador basado en marcas de tiempo, panel de inicio, reglas de tickets (prioridad y transiciones).
- Contenido educativo de los 30 días con profundidad homogénea: objetivo, tiempo estimado, explicación para principiantes, conceptos, ejemplo laboral, procedimiento paso a paso, ejercicio guiado e independiente, 4 preguntas de evaluación con explicación, errores frecuentes, resumen, criterio de finalización y recomendación de repaso.
- Laboratorio con 10 escenarios simulados y evaluación ponderada de decisiones.
- Simulador de tickets con CRUD, estados, historial, filtros y búsqueda.
- Banco de entrevistas (21 preguntas, 12 áreas, con seguimientos) y registro de postulaciones con seguimiento.
- Copia de seguridad y restauración con validación de integridad y versión de esquema.
- Interfaz WPF en español con tema oscuro por defecto y tema claro, navegación lateral de 7 módulos.
- Pruebas xUnit escritas para reglas de dominio, servicios y persistencia (incluye validación de postulaciones `JobRules`).
- **Casos de práctica para el Simulador de Help Desk** (`TicketPracticeScenarios`): 9 escenarios L1 (cuenta bloqueada, aplicación con error, equipo lento, impresora, sin internet, DNS, DHCP/APIPA, permisos y escalamiento obligatorio) con descripción del usuario, preguntas de diagnóstico, pasos sugeridos, solución/escalamiento y documentación del ticket; se cargan en el formulario desde un selector con guía consultable.
- **Eliminación de postulaciones con confirmación** (`IJobStore.DeleteApplication`): borrado en cascada transaccional (postulación + seguimientos) desde la UI con diálogo de confirmación, mensaje de cancelación y refresco; 3 pruebas de integración nuevas.
- **Guía de fortalezas y temas por repasar** en Preparación laboral: calculada desde las autoevaluaciones reales guardadas (áreas ≥4 = fortaleza, <3 = repaso, 3–4 = consolidación, pendientes sin práctica) con advertencia explícita de que es una guía subjetiva; se recalcula al instante al guardar cada respuesta.
- Pregunta q21 dedicada a la **comunicación de la solución al usuario** (validar con el usuario, explicar sin jerga, cierre documentado).
- Pruebas nuevas: `PracticeScenarioTests` (5 pruebas de integridad de los casos de práctica, incluida cobertura DNS/DHCP/IP) y las 3 de borrado en cascada en `JobStoreTests`.
- Configuración de instalador con Inno Setup 6 (`installer/rutati30.iss` + `scripts/build-installer.ps1`): instalación por usuario sin administrador; la desinstalación conserva los datos del usuario.
- Scripts de Windows: `build.ps1`, `test.ps1`, `publish.ps1`.
- Documentación: README, REQUIREMENTS, ARCHITECTURE, ROADMAP, PROGRESS, DECISIONS, TESTING, VERIFY (procedimiento de validación en Windows), USER_GUIDE, INSTALLATION, BACKUP_RESTORE.

### Limitaciones conocidas (versión 0.1.0 y ampliación sin publicar)
- Compilación, pruebas y ejecución **no verificadas**: el entorno de desarrollo no dispone del SDK .NET. Ver TESTING.md y VERIFY.md.
- Instalador: configuración lista (Inno Setup 6), pero **sin compilar ni probar** en Windows; requiere Inno Setup 6 y una VM de verificación.
- La ampliación de esta sesión (glosario, rutas, 16 escenarios, 34 preguntas, repaso espaciado) está **implementada y verificada estáticamente**, pendiente de compilación y suite en Windows. El banco de entrevistas cubre 34 preguntas en 17 áreas; puede ampliarse según necesidad.
