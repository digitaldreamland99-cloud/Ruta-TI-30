# VERIFY — Ruta TI 30 (validación en Windows)

Este documento define **qué verificar, con qué comando y con qué resultado esperado** en un equipo Windows 10/11 x64 con .NET 10 SDK. Ninguna casilla se marca sin evidencia real de ejecución.

> Estado actual (2026-10-09): **todas las casillas pendientes**. El entorno de desarrollo es Linux sin `dotnet`, sin PowerShell y sin Inno Setup (comprobado con `uname` y `command -v dotnet|pwsh|iscc|wine` → ausentes). No se simula Windows.

## 0. Comprobaciones previas del repositorio (EJECUTADAS en Linux)

| Comprobación | Resultado |
|---|---|
| Rutas referenciadas por los scripts existen (`RutaTI30.sln`, `src/RutaTI30.App/RutaTI30.App.csproj`, `installer/rutati30.iss`) | **Verificado** |
| Salida del instalador coincide con la del publicador (`artifacts\win-x64\RutaTI30.exe`) | **Verificado** (lectura de scripts) |
| Balance léxico de 75 `.cs`, XML 16/16, bindings 216/216, seguridad (SQL parametrizado, sin secretos, sin red) | **Verificado** con `scripts/verify/static-regression.py` (TESTING.md §1) |
| Contenido: 30 días ≥ 780 palabras y 4 preguntas; banco de entrevistas 34 preguntas; 16 casos de práctica; glosario 59 términos con categorías coherentes; 5 rutas | **Verificado** por análisis de los archivos fuente (`scripts/verify/edu-content-audit.py`, incluye categorías del glosario y detección de cuasi-duplicados) |
| Cableado F-05: restauración de copia notifica a `MainViewModel` → refresco de módulos con datos cargados | **Cableado verificado estáticamente** (código y DI); prueba de ejecución real pendiente en Windows (casilla 2.11) |
| Misión 2.1 (2026-10-12): balance léxico 81/81 `.cs` (tras cerrar llave faltante en `StudyTimerTickIntegrationTests.cs`), XML 16/16, bindings 216/216, `d09-wire-foundations-verification` PASSED, `git diff --check` limpio, stash 961d2fb intacto | **Verificado**; compilación y dotnet test siguen pendientes (será en Windows) |
| Misión 2.2 (2026-10-12): scripts de validación auditados (rutas, exit codes, Debug/Release, sin borrado de datos, pruebas sobre `RutaTI30.sln` con los 5 proyectos); TFMs y paquetes coherentes (versiones candidatas por confirmar con restore, DECISIONS.md D8); suite contada con precisión: 23 archivos de prueba; documentación corregida (APIs inexistentes eliminadas de TESTING.md, conteo 18→23 en casilla 1.2) | **Verificado** (lectura directa); compilación/dotnet test/recorrido WPF siguen pendientes |
| Misión 3.0 Etapa A+B (2026-10-13): diagnóstico completo (sin editar archivos en la fase A) y sistema visual ampliado en `Themes/Styles.xaml`: tokens `RadiusSm/RadiusMd`, anillo de foco `FocusRing` en botones, foco visible en campos `Field` (`IsKeyboardFocusWithin`), transición de hover 150 ms (mismo valor final que el setter, sin romper funciones), indicador de sección activa en `NavList`, variantes `Badge*(Success/Warning/Danger/Accent)` + `BadgeText*`, mensajes `Msg*(Info/Success/Warning/Error)`, `BarSuccess`, estado vacío `EmptyState/EmptyStateText`, tooltip temático, estilo `Icon` (Segoe MDL2 Assets, incluido en Windows). Solo recursos aditivos: ninguna clave existente renombrada o eliminada, ningún ViewModel/servicio/esquema alterado | **Verificado estáticamente**: `static-regression.py` → XML 16/16, léxico 81/81, bindings 216/216, sin red, SQL parametrizado (exit 0); `git diff --check` → limpio (exit 0); auditoría ad hoc → 18 claves de recurso usadas, 0 faltantes, 0 `StaticResource` fuera de orden, XML bien formado. Render real de WPF: pendiente en Windows (casillas 1.1–1.3 y §2) |
| Misión 4.0 (2026-10-13): Etapa C dashboards con accesos rápidos reales (`OpenModuleCommand` reutilizando `Navigate` + 7 CommandParameter a módulos existentes), iconos y subtítulo en las tarjetas de métricas (Inicio); Etapa D pantalla piloto de lección/cuestionario con ancho de lectura 760 px y opciones con plantilla propia (hover + borde de acento al seleccionar); Etapa E con ancho de lectura en Glosario y contexto del laboratorio; Etapa H sin contenido educativo modificado (sin defectos con evidencia suficiente); objeto `961d2fb` confirmado intacto; persistencia/esquema/respaldos/NuGet/red sin cambios | **Verificado estáticamente**: `static-regression.py` → XML 16/16, léxico 81/81, bindings 223/223 (7 command-bindings nuevos resueltos), sin red, SQL parametrizado (exit 0); `git diff --check` → limpio (exit 0); `edu-content-audit` → PASSED (exit 0); `d09-wire-foundations-verification` → PASSED (exit 0). Render real de WPF, navegación, compilación y suite: pendientes en Windows (casillas 1.1–1.3 y §2) |
| Misión 5.0 (2026-10-13) — validación final: **Fase 1 (diagnóstico) APROBADA**: `main`/`9afd8ad` sin commits/push, 33 modificados + 22 sin seguimiento según `git status --branch --short`, objeto `961d2fb` intacto (tipo `commit`), 0 artefactos previos (`*.exe`/`*.msi`/`*setup*` → 0), documentación vigente re-leída (TESTING/REQUIREMENTS). **Fases 2–5 (compilación, pruebas xUnit, validación visual WPF de 9 secciones + 7 accesos rápidos + ambos temas, publicación/instalador) BLOQUEADAS por entorno**: `dotnet`, `pwsh`, `powershell`, `iscc`, `wine`, `msbuild`, `mono` ausentes (comprobados con `command -v`); `dotnet --version` y `pwsh -File scripts/test.ps1` → «command not found»; Linux 6.1.158+ x86_64 sin capa de ejecución Windows. **Fase 6 (cierre) APROBADA (estática)**: static-regression PASSED (bindings 223/223, exit 0), edu-content-audit PASSED 30/120/34/16/10/59 (exit 0), d09-wire-foundations exit 0, `git diff --check` limpio (exit 0), trabajo previo preservado (55 entradas de estado, diff +1051/−69) | **Registrado con exit codes reales**; las fases 2–5 NO se simuladas: las casillas §1–§4 siguen «Pendiente» hasta validarse en Windows con .NET 10 SDK |
| Misión 6.0 (2026-10-13) — rediseño premium integral: kit ampliado en `Styles.xaml` (aparición discreta de mensajes 180 ms vía `MsgBase`, transición de press 120 ms solo de entrada — sin storyboards simultáneos sobre la misma propiedad); dashboard con insignia de estado semántica en vista (DataTriggers: No iniciado=Muted, En curso=Accent, Completado/Dominado=Success; el texto nombra la condición); `MsgInfo` en las tres tarjetas de resultado (Inicio/bienvenida, evaluación de lección, resultado del laboratorio); lectura 760 px en respuestas modelo y explicaciones de entrevistas; sin cambios de ViewModel/servicios/persistencia en esta misión; hallazgo menor preexistente documentado (`BoolToVis` definido solo en HomeTemplates y resuelto a nivel app) | **Verificado estáticamente**: static-regression PASSED → XML 16/16, léxico 81/81, bindings 226/226, sin red, SQL parametrizado (exit 0); auditoría de recursos: 19 claves, 0 faltantes, 0 fuera de orden; `git diff --check` limpio (exit 0). Render, animaciones en pantalla y accesibilidad real: pendientes en Windows (§1–§2) |
| Misiones 7.0–9.0 (2026-10-13) — auditoría/validación/publicación: inspección inicial sin novedades (37 mod. + 22 unp. según `git status`, `961d2fb` intacto); **M7** corrigió 2 insignias restantes con semántica comprobada contra los strings reales de los ViewModels (`LessonStateText`, `PriorityText` P1–P4) en ModuleTemplates/WorkTemplates; **M8** re-validó estáticamente (bindings 232/232 léxico 81/81, exit 0 ×4) y documentó bloqueos sin simular; **M9** no generó artefactos (find → 0) e instrucciones exactas de publicación/instalador intactas | **APROBADA (estática)**: exit 0 en static-regression/edu-content/d09/diff-check; **BLOQUEADAS** para build/test/visual/publicación (sin Windows; casillas §1–§4 «Pendiente») |
| Auditoría lingüística y de codificación (2026-10-13): 0 caracteres corruptos UTF-8/Unicode en `src/` y `tests/`; 0 tildes faltantes en palabras frecuentes; 0 dobles espacios en `Text/Content/ToolTip`; 1 marcador-like en contenido («TODO» en `LessonsBloque4.cs`) corregido a «todo»; 1 expresión ambigua («entre sí sí…» en `LabScenarios.cs`) reescrita como «sí se comunican entre sí» sin cambiar significado ni respuestas | **APROBADAS (estática)**: static-regression PASSED (exit 0), edu-content PASSED (exit 0, conteos 30/120/34/16/10/59 intactos), `git diff --check` limpio. Verificación en pantalla de textos: pendiente (§2) |

## 1. Compilación y pruebas (Windows)

```powershell
powershell -ExecutionPolicy Bypass -File scripts\build.ps1
powershell -ExecutionPolicy Bypass -File scripts\test.ps1
```

| # | Casilla | Resultado esperado | Estado |
|---|---------|--------------------|--------|
| 1.1 | `build.ps1` compila Debug y Release sin errores | exit 0 | Pendiente |
| 1.2 | `test.ps1` aprueba la suite (**23 archivos de prueba**: dominio, servicios, persistencia, respaldos, contenido, JobRules, glosario, rutas, repaso, integración de temporizador/respaldo/temas débiles/DI) | 0 fallos | Pendiente |
| 1.3 | `dotnet run --project src\RutaTI30.App` inicia y muestra la ventana principal | Ventana visible, sin error de DI | Pendiente |

Si aparecen errores: corregir el código (no las pruebas), reejecutar y registrar aquí el error real y su corrección.

## 2. Funcional (recorrido manual; app en ejecución)

Funciona sin Internet (desconecta la red antes de empezar).

| # | Casilla | Qué se observa | Estado |
|---|---------|----------------|--------|
| 2.1 | Navegación por las **9 secciones** laterales (Inicio, Mi ruta, Laboratorio, Tickets, Preparación laboral, Mi progreso, Glosario, Rutas, Configuración) | Cada sección carga sin error | Pendiente |
| 2.2 | Acceso a los **30 días** de la ruta; apertura de una lección completa | Contenido completo visible | Pendiente |
| 2.3 | Guardado/recuperación de progreso | Marcar lección → cerrar → reabrir → estado persiste | Pendiente |
| 2.4 | Temporizador: iniciar, pausar, reanudar, finalizar; cierre en curso | Tiempo pausado no cuenta; recuperación sin inflar tiempo | Pendiente |
| 2.5 | Laboratorio: resolver un escenario | Evidencias cambian según acciones; puntuación razonada | Pendiente |
| 2.6 | Tickets: crear, filtrar, cambiar estado inválido (debe rechazarse), resolver, historial | Transición inválida rechazada con mensaje | Pendiente |
| 2.7 | Entrevistas: responder y autoevaluar; guardar respuestas | Historial crece y persiste | Pendiente |
| 2.8 | Vacantes: alta con enlace no-http y fechas incoherentes (deben rechazarse) | Validación `JobRules` en la UI | Pendiente |
| 2.9 | Casos de práctica del Help Desk: cargar los 16 casos y revisar guía | Formulario se carga, aparecen preguntas de diagnóstico y pasos | Pendiente |
| 2.10 | Vacantes: eliminar una con confirmación | Borrado en cascada con diálogo; otra postulación no afectada | Pendiente |
| 2.11 | **F-05 — Restaurar una copia válida con módulos abiertos**: estar en «Inicio» o «Mi progreso» con datos visibles, luego restaurar en Configuración | Los paneles de Inicio/Progreso/Ruta muestran **los datos restaurados** sin cerrar la app (no los de la base anterior) | Pendiente |
| 2.12 | Copia de seguridad y restauración con archivo corrupto (debe rechazarse) | Datos intactos tras rechazo; no hay notificación de cambio de base | Pendiente |
| 2.13 | Búsqueda en tickets con caracteres especiales (`%`, `_`, varios espacios, símbolos) | Coincidencia literal: «%» no devuelve todos los tickets | Pendiente |

Nota de alcance: el plan funcionale define **9 módulos**; la navegación lateral ofrece **9 secciones** (Inicio; Ruta con evaluaciones; Laboratorio; Tickets; Preparación laboral con entrevistas y vacantes; Progreso; Glosario; Rutas posteriores al día 30; Configuración). Todas las funciones de los 9 módulos existen dentro de esas secciones.

## 3. Publicación

```powershell
powershell -ExecutionPolicy Bypass -File scripts\publish.ps1
```

| # | Casilla | Resultado esperado | Estado |
|---|---------|--------------------|--------|
| 3.1 | Se genera `artifacts\win-x64\RutaTI30.exe` y archivos de runtime | Carpeta completa | Pendiente |
| 3.2 | El ejecutable publicado inicia en una sesión sin SDK .NET | Ventana visible | Pendiente |
| 3.3 | Crea sus datos en `%LOCALAPPDATA%\RutaTI30\` y los recupera al reabrir | BD `ruta-ti-30.db` creada y poblada | Pendiente |

## 4. Instalador (Inno Setup 6)

```powershell
# Requiere: Inno Setup 6 (https://jrsoftware.org/isinfo.php) y 3 ejecutado
powershell -ExecutionPolicy Bypass -File scripts\build-installer.ps1
```

| # | Casilla | Resultado esperado | Estado |
|---|---------|--------------------|--------|
| 4.1 | Se genera `artifacts\installer\rutati30-0.1.0-win-x64-setup.exe` | Archivo presente | Pendiente |
| 4.2 | Instalación en **VM limpia** (no el equipo de uso diario): sin pedir administrador | Instala en `%LOCALAPPDATA%\Programs\RutaTI30` | Pendiente |
| 4.3 | La app instalada inicia y crea sus datos | Ventana + BD | Pendiente |
| 4.4 | Desinstalación: **los datos de `%LOCALAPPDATA%\RutaTI30` permanecen** | Carpeta de datos intacta | Pendiente |
| 4.5 | Reinstalación sobre datos existentes: el progreso se conserva | Migración sin pérdida | Pendiente |

## 5. Registro de resultados

Cuando se ejecute en Windows, anotar aquí: fecha, versión de Windows, versión de SDK, comandos ejecutados, salida resumida y cualquier corrección aplicada. Con ese evidencia se actualizan los estados de `REQUIREMENTS.md` (`SV` → `V`) y `TESTING.md`.
