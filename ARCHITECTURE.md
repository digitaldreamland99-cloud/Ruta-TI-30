# ARCHITECTURE — Ruta TI 30

## Visión general

Aplicación de escritorio WPF (MVVM), offline-first, sin cuentas ni servicios externos. Datos locales en SQLite dentro del perfil del usuario.

```
RutaTI30.sln
├── src/
│   ├── RutaTI30.Domain/          Modelos, enums, reglas puras (sin dependencias)
│   ├── RutaTI30.Application/     Servicios de negocio, contenido educativo, catálogos
│   ├── RutaTI30.Infrastructure/  SQLite, migraciones, repositorios, copias de seguridad
│   └── RutaTI30.App/             WPF: vistas, ViewModels, temas, navegación, DI
├── tests/
│   └── RutaTI30.Tests/           xUnit: dominio, servicios, persistencia, respaldo
├── scripts/                      build.ps1, test.ps1, publish.ps1 (Windows)
└── *.md                          Documentación
```

## Dependencias entre proyectos

```
App ──► Application ──► Domain
 │            │
 │            └──► Infrastructure ──► Domain
 └──► Infrastructure (composición en App/Bootstrap)
Tests ──► Domain, Application, Infrastructure
```

- `Domain` no depende de nadie.
- `Application` depende de `Domain` e interfaces de infraestructura definidas en `Application` cuando corresponde (p. ej. reloj, almacenes se inyectan como clases concretas de `Infrastructure` solo en la composición de la App).
- No hay dependencias circulares.

## Capas y responsabilidades

| Capa | Contenido | Prohibido aquí |
|---|---|---|
| Presentación (App) | Vistas XAML, ViewModels, temas, comandos, diálogo | Lógica de negocio, SQL |
| Aplicación | Servicios: progreso, evaluación, planificador, temporizador, panel, reglas de tickets; contenido educativo | SQL, elementos WPF |
| Dominio | Modelos y enums | Infraestructura, UI |
| Infertura (Infrastructure) | Migraciones, almacenes SQLite, respaldo/restauración | Reglas de negocio complejas |

## Persistencia

- Archivo: `%LOCALAPPDATA%/RutaTI30/ruta-ti-30.db` (WAL).
- Versión de esquema: `PRAGMA user_version`; cada migración corre en transacción.
- Todas las consultas usan parámetros (`AddWithValue`); nunca concatenan entradas del usuario.
- Operaciones múltiples (alta de ticket + historial, cierre de sesión + estado del temporizador) usan transacciones.

## Composición y arranque

1. `App.OnStartup`: se crea `ServiceCollection`, se registran reloj, almacenes, servicios, ViewModels y MainWindow.
2. Se inicializa la base de datos (migraciones) antes de mostrar la ventana; un fallo crítico se registra en archivo de log y se muestra un diálogo claro.
3. Navegación lateral: `MainViewModel` cambia `CurrentViewModel`; las plantillas de datos en `App.xaml` mapean ViewModel → vista.

## Temporizador

`StudyTimerService` calcula tiempos con diferencias reales de `IClock`, no con ticks de UI. La UI solo refresca la visualización cada segundo. Estado persistido en `timer_state`; sesiones en `study_sessions`.

## Seguridad

- Sin ejecución de procesos del sistema operativo.
- Sin escritura en registro, servicios, permisos o red.
- Sin red, telemetría ni credenciales.
- Copias de seguridad validadas (`integrity_check` + versión de esquema) antes de restaurar; se conserva copia previa del estado actual.
