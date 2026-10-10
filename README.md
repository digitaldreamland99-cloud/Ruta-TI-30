# Ruta TI 30

Aplicación de **escritorio para Windows** (C# / .NET 10 / WPF / SQLite) que prepara durante 30 días a personas principiante para buscar sus primeras oportunidades en **Soporte TI, Mesa de Ayuda L1, Service Desk y operaciones básicas**. Funciona **sin Internet, sin cuentas y sin nube**: todos los datos viven en tu perfil de usuario.

> Aviso honesto: completar este programa **no** es una certificación profesional **ni** garantiza empleo. Es una preparación de base con práctica y medición real de tu progreso.

## Qué incluye

| Módulo | Descripción |
|---|---|
| **Inicio (Mi día)** | Día del programa, actividad recomendada, tiempo estudiado, pendientes, resumen semanal, accesos rápidos. |
| **Mi ruta de aprendizaje** | 30 días de contenido real (objetivo, explicación, vocabulario, ejemplo laboral, procedimiento, errores frecuentes, ejercicios, evaluación con soluciones razonadas y criterios de aprobación). Estados: No iniciado → En curso → Completado → **Dominado mediante evaluación (≥ 80 %)**. |
| **Temporizador** | Cuenta regresiva con inicio, pausa, reanudación y finalizar; horario editable; medición por marcas de tiempo (el tiempo pausado o con la app cerrada **no** cuenta); recuperación de sesiones interrumpidas. |
| **Laboratorio de soporte** | 10 escenarios simulados (redes, DNS, accesos, impresoras, rendimiento, servicios…) donde tus decisiones cambian las evidencias y el puntaje. 100 % simulado: no toca tu equipo. |
| **Simulador de Help Desk** | Tickets con clasificación, prioridad calculada (impacto × urgencia), transiciones de estado válidas, notas, escalamiento, resolución e historial completo. **9 casos de práctica cargables** (cuentas bloqueadas, aplicaciones con error, equipos lentos, impresoras, red, DNS, DHCP, permisos, escalamiento) con preguntas de diagnóstico, pasos sugeridos y criterios de solución o escalamiento. |
| **Preparación laboral** | Banco de 21 preguntas de entrevista en 12 áreas (Service Desk, diagnóstico, Windows, redes, soporte de aplicaciones, escalada, seguridad, atención al usuario…) con respuestas orientativas, conceptos clave, errores frecuentes y criterios; respuestas personales autoevaluadas con **guía de fortalezas y temas por repasar** calculada de tus autoevaluaciones; registro local de postulaciones con estados, validación de campos, seguimientos y eliminación con confirmación. |
| **Mi progreso** | Estadísticas calculadas solo de tus datos persistidos; temas débiles; competencias demostradas; informe exportable a Markdown. |
| **Configuración** | Tema oscuro/claro, horario, duración de sesiones, umbral de aprobación, día del programa, copia de seguridad y restauración validada. |

## Estructura del repositorio

```
RutaTI30.sln
├── src/RutaTI30.Domain/          Modelos, enums y reglas puras (sin dependencias)
├── src/RutaTI30.Application/     Servicios de negocio + contenido educativo de los 30 días
├── src/RutaTI30.Infrastructure/  SQLite (Microsoft.Data.Sqlite), migraciones, almacenes, respaldos
├── src/RutaTI30.App/             Aplicación WPF (MVVM, temas, navegación)
├── tests/RutaTI30.Tests/         Pruebas xUnit (dominio, servicios, persistencia, respaldos)
├── scripts/                      build.ps1, test.ps1, publish.ps1 (Windows)
└── *.md                          Documentación completa
```

## Requisitos

- Windows 10/11 x64.
- [.NET 10 SDK](https://dotnet.microsoft.com/download) solo para compilar (ver INSTALLATION.md).
- No necesita Internet para funcionar una vez instalada.

## Compilar y ejecutar (Windows)

```powershell
# Compilar Debug y Release
powershell -ExecutionPolicy Bypass -File scripts\build.ps1

# Ejecutar pruebas
powershell -ExecutionPolicy Bypass -File scripts\test.ps1

# Ejecutar desde el código
dotnet run --project src\RutaTI30.App

# Publicar para distribuir (no es un instalador)
powershell -ExecutionPolicy Bypass -File scripts\publish.ps1
```

## Datos y privacidad

- Base de datos: `%LOCALAPPDATA%\RutaTI30\ruta-ti-30.db` (SQLite con esquema versionado).
- Copias de seguridad: `%LOCALAPPDATA%\RutaTI30\backups\`.
- Sin telemetría, sin anuncios, sin envío de datos, sin credenciales almacenadas.
- Guía de respaldo y restauración: [BACKUP_RESTORE.md](BACKUP_RESTORE.md).

## Estado del proyecto

Versión **0.1.0 (en desarrollo)**. La compilación y las pruebas deben ejecutarse en Windows con .NET 10 SDK; el entorno de desarrollo actual no disponía de dicho SDK, por lo que las verificaciones están **pendientes de ejecución** (detalle en [TESTING.md](TESTING.md)). Estado por requisitos en [REQUIREMENTS.md](REQUIREMENTS.md); plan de fases en [ROADMAP.md](ROADMAP.md).

## Documentación

- [USER_GUIDE.md](USER_GUIDE.md) — guía de usuario en español.
- [INSTALLATION.md](INSTALLATION.md) — instalación, ejecución, actualización y publicación.
- [ARCHITECTURE.md](ARCHITECTURE.md) — arquitectura y decisiones técnicas.
- [DECISIONS.md](DECISIONS.md) — decisiones técnicas y motivos.
- [TESTING.md](TESTING.md) — pruebas escritas, ejecutadas y pendientes.
- [VERIFY.md](VERIFY.md) — procedimiento de validación en Windows (comandos y casillas).
- [BACKUP_RESTORE.md](BACKUP_RESTORE.md) — respaldo y restauración.
