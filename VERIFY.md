# VERIFY — Ruta TI 30 (validación en Windows)

Este documento define **qué verificar, con qué comando y con qué resultado esperado** en un equipo Windows 10/11 x64 con .NET 10 SDK. Ninguna casilla se marca sin evidencia real de ejecución.

> Estado actual (2026-10-09): **todas las casillas pendientes**. El entorno de desarrollo es Linux sin `dotnet`, sin PowerShell y sin Inno Setup (comprobado con `uname` y `command -v dotnet|pwsh|iscc|wine` → ausentes). No se simula Windows.

## 0. Comprobaciones previas del repositorio (EJECUTADAS en Linux)

| Comprobación | Resultado |
|---|---|
| Rutas referenciadas por los scripts existen (`RutaTI30.sln`, `src/RutaTI30.App/RutaTI30.App.csproj`, `installer/rutati30.iss`) | **Verificado** |
| Salida del instalador coincide con la del publicador (`artifacts\win-x64\RutaTI30.exe`) | **Verificado** (lectura de scripts) |
| Balance léxico de 65 `.cs`, XML 15/15, bindings 184/184, seguridad (SQL parametrizado, sin secretos, sin red) | **Verificado** en revisiones estáticas previas (TESTING.md §1) |
| Contenido: 30 días ≥ 780 palabras y 4 preguntas; banco de entrevistas 20 preguntas | **Verificado** por análisis de los archivos fuente |

## 1. Compilación y pruebas (Windows)

```powershell
powershell -ExecutionPolicy Bypass -File scripts\build.ps1
powershell -ExecutionPolicy Bypass -File scripts\test.ps1
```

| # | Casilla | Resultado esperado | Estado |
|---|---------|--------------------|--------|
| 1.1 | `build.ps1` compila Debug y Release sin errores | exit 0 | Pendiente |
| 1.2 | `test.ps1` aprueba la suite (14 archivos: dominio, servicios, persistencia, respaldos, contenido, JobRules) | 0 fallos | Pendiente |
| 1.3 | `dotnet run --project src\RutaTI30.App` inicia y muestra la ventana principal | Ventana visible, sin error de DI | Pendiente |

Si aparecen errores: corregir el código (no las pruebas), reejecutar y registrar aquí el error real y su corrección.

## 2. Funcional (recorrido manual; app en ejecución)

Funciona sin Internet (desconecta la red antes de empezar).

| # | Casilla | Qué se observa | Estado |
|---|---------|----------------|--------|
| 2.1 | Navegación por las **7 secciones** laterales (Inicio, Mi ruta, Laboratorio, Tickets, Preparación laboral, Mi progreso, Configuración) | Cada sección carga sin error | Pendiente |
| 2.2 | Acceso a los **30 días** de la ruta; apertura de una lección completa | Contenido completo visible | Pendiente |
| 2.3 | Guardado/recuperación de progreso | Marcar lección → cerrar → reabrir → estado persiste | Pendiente |
| 2.4 | Temporizador: iniciar, pausar, reanudar, finalizar; cierre en curso | Tiempo pausado no cuenta; recuperación sin inflar tiempo | Pendiente |
| 2.5 | Laboratorio: resolver un escenario | Evidencias cambian según acciones; puntuación razonada | Pendiente |
| 2.6 | Tickets: crear, filtrar, cambiar estado inválido (debe rechazarse), resolver, historial | Transición inválida rechazada con mensaje | Pendiente |
| 2.7 | Entrevistas: responder y autoevaluar; guardar respuestas | Historial crece y persiste | Pendiente |
| 2.8 | Vacantes: alta con enlace no-http y fechas incoherentes (deben rechazarse) | Validación `JobRules` en la UI | Pendiente |
| 2.9 | Casos de práctica del Help Desk: cargar 9 casos y revisar guía | Formulario se carga, aparecen preguntas de diagnóstico y pasos | Pendiente |
2.10 | Vacantes: alta con enlace no-http y fechas incoherentes (deben rechazarse); eliminar una con confirmación | Validación `JobRules` en la UI; borrado en cascada con diálogo | Pendiente |
2.12 | Copia de seguridad y restauración con archivo corrupto (debe rechazarse) | Datos intactos tras rechazo | Pendiente |

Nota de alcance: el plan funcionale define **9 módulos**; en la interfaz se agrupan en **7 secciones** de navegación (Inicio; Ruta con evaluaciones; Laboratorio; Tickets; Preparación laboral con entrevistas y vacantes; Progreso; Configuración). Todas las funciones de los 9 módulos existen dentro de esas 7 secciones.

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
