# DECISIONS — Ruta TI 30

Registro de decisiones técnicas relevantes y sus motivos.

## D1. Arquitectura en cuatro proyectos

- `RutaTI30.Domain`: modelos, enums, reglas puras (sin dependencias externas).
- `RutaTI30.Application`: servicios de negocio, catálogos de contenido, reglas de evaluación/progreso/temporizador.
- `RutaTI30.Infrastructure`: SQLite (`Microsoft.Data.Sqlite`), migraciones, repositorios, copias de seguridad.
- `RutaTI30.App`: WPF + MVVM (presentación).
- `RutaTI30.Tests`: xUnit sobre Domain/Application/Infrastructure.

Motivo: separación de responsabilias exigida por el enunciado, sin sobrediseñar (no se crean interfaces por cada clase; solo donde hay más de una implementación real: `IClock`, catálogos e infraestructura sustituible en pruebas).

## D2. MVVM sin CommunityToolkit.Mvvm

Se implementa un `ObservableObject` base y `RelayCommand`/`AsyncRelayCommand` propios (~120 líneas) en `RutaTI30.App/Mvvm/`.

Motivo: el entorno actual no permite verificar versiones de paquetes NuGet (`dotnet` ausente). Minimizar dependencias no verificables reduce el riesgo de fallo de restore en la primera compilación. Si se confirma la versión correcta, migrar a CommunityToolkit.Mvvm es un cambio de bajo riesgo y quedará documentado.

## D3. Persistencia: SQLite con `Microsoft.Data.Sqlite`

Única dependencia de datos. Esquema versionado con `PRAGMA user_version`; migraciones en transacción; consultas 100 % parametrizadas; modo WAL.

Motivo: permitido por el enunciado, sin ORM (evita dependencias pesadas y pérdida de control sobre el esquema).

## D4. Ubicación de datos

`%LOCALAPPDATA%/RutaTI30/ruta-ti-30.db` (perfil del usuario, independiente de la instalación). Copias de seguridad en `%LOCALAPPDATA%/RutaTI30/backups/`.

## D5. Medición del temporizador por marcas de tiempo

El servicio calcula tiempo con `IClock.UtcNow` (diferencias reales entre eventos), no con un contador incrementado por tick visual. Se persiste el estado en cada transición (iniciar/pausar/reanudar/finalizar) y un `persisted_tick_utc` como máximo cada 30 s. Al recuperar una sesión "en curso" tras un cierre inesperado, el tiempo que no pudo verificarse (entre el último tick persistido y el cierre) **no** se cuenta como estudio efectivo.

Motivo: regla 6 del enunciado — no contar tiempo no verificado ni duplicarlo.

## D6. Día académico no ligado al calendario

`current_day` se guarda en preferencias; solo cambia por acción explícita del usuario (botón "Avanzar de día"). La fecha calendario se muestra aparte.

Motivo: exigido por el enunciado; evita perder el ritmo si el usuario deja de estudiar.

## D7. Umbral de dominio

- Completado: el usuario marca lectura y ejercicios realizados.
- Dominado: intento de evaluación con puntaje ≥ 80 % (umbral configurable en preferencias) en la evaluación de la lección.
- Completar una lectura nunca otorga "Dominado".

## D8. Versiones de paquetes (candidatas, pendientes de confirmar)

| Paquete | Versión candidata | Uso |
|---|---|---|
| Microsoft.Data.Sqlite | 10.0.0 | Persistencia |
| Microsoft.Extensions.DependencyInjection | 10.0.0 | Composición |
| Microsoft.Extensions.Logging | 10.0.0 | Registro técnico a archivo |
| xunit / xunit.runner.visualstudio / Microsoft.NET.Test.Sdk | 2.9.3 / 3.x / 17.14.x | Pruebas |

Motivo de la cautela: sin `dotnet restore` no es posible verificar disponibilidad exacta. Si el restore falla, ajustar versiones a las disponibles en la fuente configurada y documentar el cambio aquí.

## D9. Laboratorio 100 % simulado

Los escenarios ejecutan "acciones" que solo producen evidencias simuladas almacenadas; no se invoca ningún proceso del sistema operativo, no se escribe el registro de Windows, no se modifican servicios ni permisos.

## D10. SLA didácticos configurables

Los objetivos de atención de los ejemplos son valores de práctica editables en preferencias; la UI y la documentación indican que no son estándares universales.

## D11. Instalador con Inno Setup 6 (per-user, sin admin)

Se prepara `installer/rutati30.iss` con Inno Setup 6 en lugar de MSIX o NSIS: es gratuito, se compila con un solo comando (`ISCC.exe`), no requiere certificado para pruebas locales y permite instalación **por usuario** (`PrivilegesRequired=lowest`), coherente con el requisito de no requerir privilegios de administrador. La desinstalación **conserva** `%LOCALAPPDATA%\RutaTI30` a propósito: los datos del usuario no se eliminan sin acción explícita. El script queda como configuración reproducible; su ejecución y prueba en una VM limpia siguen pendientes en Windows (documentado en INSTALLATION.md).
