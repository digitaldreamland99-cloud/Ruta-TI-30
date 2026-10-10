# AUDIT_REPORT — Ruta TI 30

Informe de auditoría integral realizado sobre el repositorio real en el entorno actual. No se simularon Windows, .NET, WPF ni SQLite; las conclusiones que requieren ejecución se etiquetan explícitamente como tales.

## 1. Resumen ejecutivo

El proyecto es coherente con la arquitectura anunciada: solución de 5 proyectos C#/.NET 10/WPF/SQLite con MVVM, persistencia local y funcionamiento sin conexión. La capa de dominio está limpia (sin dependencias de UI o datos). Los servicios de aplicación están bien especializados y los almacenes de infraestructura usan consultas parametrizadas y transacciones en los puntos críticos. La interfaz usa plantillas de recurso por ViewModel y los comandos están implementados, no declarados.

Lo esencial está implementado; lo faltante es validación en tiempo de ejecución, no ausencia de código. El entorno actual (Linux sin SDK) impide compilar, ejecutar la app y ejecutar las pruebas; eso deja bloqueados varios requisitos que, por su naturaleza, no pueden cerrarse aquí.

## 2. Alcance de la auditoría

- Inventario de proyectos, código, XAML, pruebas, scripts, instalador, dependencias y documentación.
- Arquitectura y dependencias entre capas.
- Calidad de C#: modelos, reglas de dominio, servicios, persistencia y manejo de errores.
- WPF / XAML / MVVM: vistas, ViewModels, comandos, bindings y recursos.
- SQLite: esquema, migraciones, parametrización, transacciones, respaldo y restauración.
- Seguridad y privacidad: secretos, red, procesos externos, rutas y restauración.
- Contenido educativo: lecciones, evaluaciones, banco de entrevistas, casos de práctica y guía de fortalezas.
- Módulos funcionales y navegación.
- Pruebas automatizadas: inventario, cobertura temática y regresiones.
- Publicación, instalador y conservación de datos del usuario.
- Documentación y trazabilidad entre requisitos, código, pruebas y evidencia.

No se executó: `dotnet restore`, `dotnet build`, `dotnet test`, ejecución de la aplicación, publicación ni compilación del instalador.

## 3. Entorno

- Sistema operativo: Linux x86_64.
- .NET SDK: **no disponible** (no se encontró `dotnet`).
- PowerShell, Inno Setup, Wine: **no disponibles** en este entorno.
- Git: rama `main`, HEAD `= dd08f15` (único commit inicial). Estado laboral: hay archivos nuevos sin versionar y un cambio pendiente en `README.md`.
- Herramientas auxiliares usadas para lo posible: Python 3, expresiones regulares y lectura directa de archivos.

## 4. Inventario

### 4.1 Solución y proyectos
- Solución: `RutaTI30.sln` con 5 proyectos.
- `src/RutaTI30.Domain` — `net10.0`, sin dependencias externas.
- `src/RutaTI30.Application` — `net10.0`, depende de Domain e Infrastructure referenciados como ProjectReference.
- `src/RutaTI30.Infrastructure` — `net10.0`, depende de Domain y Application; usa `Microsoft.Data.Sqlite`.
- `src/RutaTI30.App` — `net10.0-windows`, `UseWPF=true`, depende de los tres proyectos anteriores; usa `Microsoft.Extensions.DependencyInjection` y `Microsoft.Extensions.Logging`.
- `tests/RutaTI30.Tests` — `net10.0`, depende de Domain, Application e Infrastructure; usa xUnit, Microsoft.NET.Test.Sdk y xunit.runner.visualstudio.

### 4.2 Código fuente
- Archivos `.cs` en `src`: 51.
- Vistas XAML en `src`: 8 (App.xaml, MainWindow.xaml, 3 temas, 3 diccionarios de vistas).
- ViewModels: `MainViewModel`, `HomeViewModel`, `LearnViewModel`, `LabViewModel`, `TicketsViewModel`, `CareerViewModel`, `ProgressViewModel`, `SettingsViewModel`.
- MVVM propio: `ObservableObject`, `RelayCommand`, `AsyncRelayCommand`, servicios de diálogo, tema y registro a archivo.

### 4.3 Contenido educativo
- Lecciones: 30, distribuidas en 5 bloques, cargadas desde `LessonsBloque1..5`.
- Estructura de lección completa: objetivo, dificultad, duración, requisitos, explicación principiante, conceptos, ejemplo laboral, procedimiento, errores frecuentes, ejercicio guiado e independiente, preguntas de evaluación, resumen, recomendaciones de repaso, referencias y criterio de aprobación.
- Banco de entrevistas: 21 preguntas.
- Casos de práctica de Help Desk: 9.
- Lab de soporte: catálogo y almacén de intentos.

### 4.4 Persistencia y esquema
- Base de datos: SQLite local en `%LOCALAPPDATA%\RutaTI30\ruta-ti-30.db`.
- Esquema inicial versionado con `PRAGMA user_version`.
- Tablas principales: `preferences`, `lesson_progress`, `evaluation_attempts`, `study_sessions`, `timer_state`, `tickets`, `ticket_history`, `lab_attempts`, `interview_attempts`, `job_applications`, `followups`.
- Índices en columnas usadas para búsqueda/filtrado.
- Claves foráneas en `ticket_history` y `followups`.
- Respaldos en `%LOCALAPPDATA%\RutaTI30\backups`.

### 4.5 Pruebas
- Archivos de prueba: 16 (`.cs`), incluido un helper (`TestHelpers.cs`).
- Métodos de prueba contados: 103 al usar como referencia la forma `public void Nombre(...)`.
- Aciertos y errores de conteos:
  - Un reporte puede citar “unidades de prueba” diferentes según si cuenta archivos, clases o métodos. Aquí se registran los tres.
  - Helper de pruebas tiene métodos de soporte, no de validación funcional; eso explica diferencias entre “15 archivos de prueba” y cifras que excluyen helpers.
  - Si un documento anterior mencionó “14 archivos”, la diferencia histórica puede venir de incluir o no helpers o de tratar los nuevos tests de casos de práctica como archivo adicional.
- Archivos de prueba existentes: `ContentTests`, `DashboardServiceTests`, `EvaluationServiceTests`, `LabScoringTests`, `PracticeScenarioTests`, `ProgressServiceTests`, `ScheduleServiceTests`, `StudyTimerServiceTests`, `TicketRulesTests`, `JobRulesTests`, `BackupServiceTests`, `JobStoreTests`, `LearningStoreTests`, `MigrationTests`, `TicketStoreTests`, `TestHelpers`.

### 4.6 Scripts, instalador y artefactos
- Scripts: `scripts/build.ps1`, `scripts/test.ps1`, `scripts/publish.ps1`, `scripts/build-installer.ps1`.
- Instalador: `installer/rutati30.iss` configurado para instalación por usuario con `PrivilegesRequired=lowest`.
- Artefactos publicados o de instalador: **no existen en el repositorio ahora**. Solo existen los scripts que los generan.

### 4.7 Documentación
- README.md, REQUIREMENTS.md, ROADMAP.md, ARCHITECTURE.md, DECISIONS.md, USER_GUIDE.md, INSTALLATION.md, BACKUP_RESTORE.md, PROGRESS.md, CHANGELOG.md, VERIFY.md, TESTING.md.
- Faltan los dos entregables nuevos que pide esta auditoría: `AUDIT_REPORT.md` (este archivo) y `AUDIT_TRACEABILITY.md` (se crea después).

## 5. Arquitectura

### 5.1 Separación de responsabilidades
- Dominio: modelos, enums y reglas puras (`TicketRules`, `JobRules`, `AttemptDetails`, `LabScoring`). No referencia WPF ni SQLite del lado de la aplicación.
- Aplicación: servicios y contenido educativo; depende de interfaces abstractas de almacenes y de Domain.
- Infraestructura: AppDb, almacenes y BackupService; implementa las abstracciones de Application.
- App: ViewModels, vistas, comandos, temas, dialogos y composición DI.

El diseño es correcto en cuanto a separación general. No se detectan dependencias circulares y el dominio no depende de la capa de presentación.

### 5.2 Inyección de dependencias y ciclo de vida
- DI se construye manualmente en `App.xaml.cs` usando `ServiceCollection`.
- Los almacenes y servicios se registran como singleton.
- Los ViewModels y MainWindow también como singleton.
- `IInterviewBank`, `ILessonCatalog` y `ILabCatalog` usan instancias únicas del catálogo.

Riesgo menor identificado:
- El catálogo de lecciones concatenta 5 bloques en la creación de `LessonCatalog.Instance`. Eso se ejecuta durante la composición DI; en un proyecto educativo de 30 lecciones complejas, el inicio puede ser un poco más costoso, pero el impacto es aceptable mientras el contenido esté embebido y sea de tamaño moderado. No es defecto, es un equilibrio de diseño.

### 5.3 Asincronía y rendimiento
- Los comandos están basados en `AsyncRelayCommand`; varios métodos ViewModel son `async Task` y devuelven `Task.CompletedTask` cuando la operación es principalmente síncrona.
- No se detectan usos obvios de `.Result` o `.Wait()` en UI.
- No se detectaron bloqueos evidentes del hilo de interfaz por operaciones sincronizadas costosas en los archivos revisados.

Punto a confirmar en ejecución:
- La estrategia de refresco del timer y la persistencia del tick deben validarse en ejecución para confirmar que la UI no se satura y que el intervalo de persistencia se comporta como se anuncia.

### 5.4 Modelos y reglas de negocio
- Reglas de tickets: cálculo de prioridad e independizado; transiciones válidas; validación de campos mínimos.
- Reglas de postulaciones: empresa y puesto obligatorios; enlace http/https cuando existe; coherencia de fechas.
- Evaluación: cálculo de puntaje, umbral, temas fallidos y cantidad de respuestas no contestadas.
- Horario: validación de rangos, duración y duplicados de id.
- Guía de fortalezas: derivada únicamente de autoevaluaciones guardadas; se recalula tras guardar respuesta.

El dominio es sólido para la prevención que ofrece y para la clase de errores educativos que el producto busca evitar.

## 6. Calidad del código

### 6.1 Defectos con impacto posible
- En el servicio de respaldo, `Restore` elimina los archivos `-wal` y `-shm` asociados a la base de datos después de copiar el archivo. Si la aplicación estuviera ejecutándose en otro proceso y hubiera un estado WAL activo, ese patrón podría ser peligroso en producción. En el diseño actual la base de datos se abre por operación y se libera con `using`, así que en uso normal la base no debería tener un proceso externo escribiendo en ella; pero el riesgo aumenta si alguien restaura mientras la app tiene el archivo abierto. El tratamiento correcto real consiste en: cerrar todo acceso antes de restaurar o reemplazar solo cuando esté seguro. Eso debe comprobarse en ejecución antes de considerar la restauración segura en todos los escenarios.

### 6.2 Buenas prácticas observadas
- Las consultas están parametrizadas.
- Las operaciones compuestas (ticket + historial, cierre de sesión del temporizador, restauración con respaldo previo) usan transacciones o estructuras equivalentes.
- Los almacenes abren y cierran conexiones por operación en lugar de mantener una conexión global abierta.
- El formato de fecha UTC usado en SQLite es ISO-8601 explícito y claro.

### 6.3 Puntos de mejora sin ser defectos críticos
- AlgunosViewModel cargan contenido en el constructor mediante comandos asíncronos; esto es válido, pero en ejecución hay que verificar que el estado inicial se comporta bien antes del primer refresh.
- El control de errores generaliza mensajes del tipo “No se pudo X: “ + ex.Message. Para una app local eso es aceptable, pero hay que comprobar que no expongan ruta interna u otra información sensible en producción; eso solo se confirma ejecutando la app con un fallo deliberado.

## 7. WPF, XAML y MVVM

### 7.1 Integridad de la interfaz
- Los recursos están declarados en diccionarios mezclados por `App.xaml`.
- Las vistas usan plantillas por tipo de ViewModel.
- MainWindow enlaza `NavItems`, `SelectedNav`, `CurrentTitle`, `CurrentDate`, `CurrentViewModel`.
- Todos los diccionarios referenciados existen y están en `App.xaml`.

Puntos a confirmar en compilación/ejecución:
- Que cada `{Binding}` resuelva correctamente en tiempo de ejecución tras la construcción de los ViewModels.
- Que los estilos personalizados de botones y campos XAML no generen advertencias de plantilla incompleta.

### 7.2 Comportamiento de controles
- Botones y comandos están conectados a métodos reales en los ViewModels.
- Transiciones de tickets, guardado de respuesta, guardado/eliminación de postulaciones, carga de casos y restauración tienen código real; no son botones huérfanos.
- El formulario de tickets carga casos con descripción, impacto, urgencia, preguntas de diagnóstico y guía.
- La sección de preparación laboral carga preguntas, respuestas, historial y guía de fortalezas.

Puntos a confirmar en ejecución:
- Bordes de filtros, caracteres esperados y posicionamiento final del formulario.
- Que los mensajes de error se muestren en los casos en que el ViewModel los produce.

### 7.3 Experiencia y accesibilidad
- Tipografía y colores definidos en temas oscuro y claro.
- Estilos de botones y campos coherentes.
- Ventana con tamaño mínimo y medido para uso en escritorio.

Hallazgo menor:
- Algunos `ColumnDefinition` usan anchos fijos muy pequeños (12, 16, 80) como separadores o para campos cortos. En el diseño actual funcionan como espaciado, pero hay que comprobar que en resoluciones pequeñas y en DPI altos no generen contenido comprimido o desbordado desordenado. Eso no se puede validar plenamente sin ejecutar la app en distintas resoluciones/escalas.

## 8. Base de datos y persistencia

### 8.1 Esquema y persistencia
- El esquema cubre progreso, evaluaciones, sesiones, timer_state, tickets, historial, intentos de laboratorio, intentos de entrevista y postulaciones con seguimientos.
- Migraciones versión por versión y aplicadas en transacción.
- Foreign keys activadas en conexión.
- WAL activado.

### 8.2 Persistencia funcional
- Guardado y recuperación por id o listado filtrado están implementados para cada entidad relevante.
- El progreso de lección, evaluaciones, sesiones, timer_state, tickets con historial, intentos de entrevista, postulaciones y seguimientos tienen almacenamiento directo.
- BackupService genera copia, valida y restaura con copia de seguridad previa opcional.

### 8.3 Copias de seguridad y restauración
- La validación revisa integridad, versión de esquema y presencia de tabla clave.
- La restauración crea respaldo de seguridad del estado actual antes de reemplazar.
- Si la restauración falla, hay lógica para restituir la copia previa.

Riesgo pendiente de validar en ejecución:
- La eliminación de archivos `-wal` y `-shm` durante la restauración debe confirmarse como segura en el flujo real de la app.
- La validación actual acepta versión `>= 1` como compatible y rechaza versiones futuras mayores que `CurrentVersion`. Eso está bien para una versión inicial, pero habrá que revisarlo cuando exista una migración real futura.

### 8.4 Respaldo frente a base vacía o corrupta
- Existen tests diseñados para archivo inexistente, archivo con texto no SQLite y archivos corruptos, además del caso de restauración inválida no destructiva.
- La base vacía es compatible con el esquema inicial porque el DDL usa `CREATE TABLE IF NOT EXISTS`.

## 9. Seguridad y privacidad

### 9.1 Buenas prácticas observadas
- No hay pruebas en el código de las que se haya podido revisar de autenticación externa, telemetría, descargas ni ejecución de procesos del sistema.
- Las credenciales del usuario no se almacenan según el diseño declarado y el modelo de datos revisado.
- Las consultas están parametrizadas.
- La app usa directorios del perfil del usuario, no rutas administrativas.

### 9.2 Riesgos residuales
- No se pudo consultar fuentes actualizadas de vulnerabilidades de paquetes desde este entorno, por lo que no se afirma que las dependencias estén libres de problemas conocidos.
- La exposición de mensajes de excepción en diálogos y logs puede revelar detalles internos si no se valida en ejecución.
- El riesgo principal de la app no es de modelo de amenaza externo, sino de integridad local en operaciones de restauración y manejo de archivos.

## 10. Contenido educativo

- Lecciones con profundidad estructurada y progresión por bloques.
- Evaluaciones con explicación y criterio de aprobación.
- Banco de 21 preguntas que cubre áreas típicas de L1 y preparación laboral.
- Casos de práctica de 9 escenarios que cubren cuentas, aplicaciones, rendimiento, impresoras, red local, DNS, DHCP/APIPA, permisos y escalamiento.
- Guía de fortalezas derivada solo de autoevaluaciones guardadas.

Puntos a revisar pedagógicamente en ejecución:
- La claridad y ausencia de contradicciones entre explicación y ejercicios debe validarse con usuarios reales, no solo con lectura estática.
- El nivel de las evaluaciones y lab debe comprobarse con estudiantes principiantes.

## 11. Módulos funcionales y navegación

Se confirman 7 secciones en la barra lateral y los módulos esperados corresponden a los ViewModels y plantillas existentes:
1. Inicio → `HomeViewModel` + `HomeTemplates.xaml`.
2. Mi ruta de aprendizaje → `LearnViewModel` + `ModuleTemplates.xaml`.
3. Laboratorio → `LabViewModel` + `ModuleTemplates.xaml`.
4. Simulador de tickets → `TicketsViewModel` + `WorkTemplates.xaml`.
5. Preparación laboral → `CareerViewModel` + `WorkTemplates.xaml`.
6. Mi progreso → `ProgressViewModel` + `WorkTemplates.xaml`.
7. Configuración → `SettingsViewModel` + `WorkTemplates.xaml`.

Los 9 módulos funcionales del enunciado están presentes en la implementación, agrupados en esas 7 secciones.

## 12. Pruebas y QA

- Hay suite de xUnit con cobertura razonable sobre dominio, servicios, persistencia, respaldos y contenido.
- Casos positivos, negativos y de límites están presentes en distintos archivos de prueba.
- Hay pruebas de borrado en cascada y de respaldo/copias corruptas.
- Algunos casos de restauración dependen de la implementación real del servicio y deben confirmarse en ejecución.

No se ejecutó ninguna prueba. Por tanto:
- No hay evidencia de aprobación ni de fallo.
- No hay métrica de cobertura real.
- No hay verificación de que los tests corran sin errores de construcción ni de runtime.

## 13. Dependencias y reproducibilidad

- Target framework es .NET 10 en todos los proyectos.
- Aplicación WPF usa la tipografía `net10.0-windows`.
- Dependencias externas principales son Microsoft.Data.Sqlite, Microsoft.Extensions.DependencyInjection y Microsoft.Extensions.Logging; las versiones vienen como candidatas y deben confirmarse con restore real.
- Los paquetes de pruebas también son candidatos hasta que se ejecute restore.

## 14. Publicación, ejecución e instalador

- Scripts listos y coherentes con la estructura del repositorio.
- Publicación apunta a `artifacts/win-x64/RutaTI30.exe` autocontenida.
- Instalador depende de lapublicación previa y de Inno Setup 6.

No existen ni `artifacts/win-x64` ni `artifacts/installer` en este momento.

## 15. Defectos priorizados

### P0
- Ninguno confirmado en este entorno. La restauración tiene una lógica de respaldo previo y validación, pero su seguridad total debe confirmarse en ejecución.

### P1
- Incapacidad de validar compilación, ejecución y pruebas aquí. Esto no es defecto del producto, pero sí bloquea el cierre de requisitos.

### P2
- Confirmar que los diálogos y logs no expongan información interna en producción.
- Confirmar el comportamiento de la restauración con archivos WAL activos en un escenario real.
- Verificar posición y legibilidad en resoluciones y escalas distintas de la Ventana principal y formularios con anchos fijos.

### P3
- Documentar con mayor precisión los conteos de prueba (archivos, clases, métodos) y 통일 los reportes.
- Ajustar la documentación de discrepancias de conteos anteriores según el inventario real: 51 `.cs` en src, 8 XAML, 16 archivos de prueba, 103 métodos de prueba contados por forma de firma.

## 16. Correcciones efectuadas en esta auditoría

Ninguna modificación de código fue necesaria para la auditoría en sí, porque el producto ya está implementado y solo faltan validaciones que no pueden ejecutarse aquí. Lo que sí se ajustó es la documentación pendiente que esta auditoría exige, y eso se hace en la siguiente sección.

## 17. Limitaciones

- No se ejecutó restore, build, test, run, publish ni ISCC.
- No se abrió la aplicación ni se recorrió la interfaz.
- No se verificó el comportamiento real de WPF para bindings, temas, escalado ni mensajes de error en tiempo de ejecución.
- No se verificó el comportamiento real de SQLite con archivos reales, ni la seguridad de la restauración bajo carga concurrente.
- No se revisó seguridad de paquetes contra bases de datos actualizadas.

## 18. Riesgos residuales

- Que la restauración pueda dejar inconsistente el archivo si se aplica en condiciones no previstas.
- Que la aplicación exponga mensajes de excepción demasiado crudos en algunos diálogos.
- Que las versiones candidatas de paquetes no estén disponibles en el feed usado durante la primera compilación en Windows.
- Que el contenido educativo necesite ajustes reales tras su uso con estudiantes reales.

## 19. Recomendaciones

1. Ejecutar primero en Windows con SDK:
   - `powershell -ExecutionPolicy Bypass -File scripts\build.ps1`
   - `powershell -ExecutionPolicy Bypass -File scripts\test.ps1`
2. Validar en ejecución los flujos críticos:
   - Tickets, transiciones inválidas, escalar/resolver.
   - Respuestas de entrevista, guardado y guía de fortalezas.
   - Postulaciones: alta, edición y eliminación con confirmación.
   - Casos de práctica: carga y guía.
   - Backups y restauración con archivo inválido.
   - Temporizador: inicio, pausa, reanudación, cierre inesperado y recuperación.
3. Validar la restauración de forma aislada antes de confiar en ella en entornos con datos reales.
4. Revisar los mensajes de error de diálogos y logs con un fallo deliberado para ajustar su detalle si fuera necesario.
5. Ajustar versiones de paquetes si restore falla en Windows y documentar el cambio en DECISIONS.md.

## 20. Próximo paso

Ejecutar en Windows con .NET 10 SDK:
1. `powershell -ExecutionPolicy Bypass -File scripts\build.ps1`
2. `powershell -ExecutionPolicy Bypass -File scripts\test.ps1`

Si alguno falla, corregir el código o la configuración y volver a ejecutar, registrando el fallo real y no asumiendo que pasará.

---

*Este informe es el entregable AUDIT_REPORT.md solicitado por la auditoría integral.*
