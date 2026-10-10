# CHANGELOG — Ruta TI 30

Formato: [Keep a Changelog] en español. Versión inicial **0.1.0 (en desarrollo)**.

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

### Limitaciones conocidas (versión 0.1.0)
- Compilación, pruebas y ejecución **no verificadas**: el entorno de desarrollo no dispone del SDK .NET. Ver TESTING.md.
- Instalador: configuración lista (Inno Setup 6), pero **sin compilar ni probar** en Windows; requiere Inno Setup 6 y una VM de verificación.
- El banco de entrevistas cubre 21 preguntas en 12 áreas; puede ampliarse según necesidad.
