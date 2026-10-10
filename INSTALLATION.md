# INSTALLATION — Ruta TI 30

> Para el checklist completo de validación tras instalar (compilación, funcional, publicación, instalador), usa [VERIFY.md](VERIFY.md).

## 1. Requisitos

| Componente | Detalle |
|---|---|
| Sistema | Windows 10 (1809+) o Windows 11, x64 |
| Para compilar | .NET 10 SDK (https://dotnet.microsoft.com/download) |
| Para ejecutar (versión publicada self-contained) | Ninguno adicional (incluye el runtime) |
| Internet | No requerido para ejecutar la aplicación |

## 2. Compilar desde el código (desarrollo)

```powershell
git clone <repositorio>
cd Ruta-TI-30
powershell -ExecutionPolicy Bypass -File scripts\build.ps1
```

El script restaura paquetes, compila **Debug** y **Release** y devuelve código de salida distinto de cero si algo falla.

## 3. Ejecutar en modo desarrollo

```powershell
dotnet run --project src\RutaTI30.App
```

La primera ejecución crea la base de datos y aplica las migraciones en `%LOCALAPPDATA%\RutaTI30\`.

## 4. Ejecutar pruebas

```powershell
powershell -ExecutionPolicy Bypass -File scripts\test.ps1
```

## 5. Publicación para Windows x64

```powershell
powershell -ExecutionPolicy Bypass -File scripts\publish.ps1
```

Salida: `artifacts\win-x64\RutaTI30.exe` (autocontenida: incluye el runtime de .NET).

**Diferencias importantes (no confundir):**

| Concepto | Qué es | Estado |
|---|---|---|
| Compilación | Resultado de `build` (Debug/Release) | Script listo; ejecutar en Windows |
| Publicación | Carpeta `artifacts\win-x64` ejecutable | Script listo; ejecutar en Windows |
| Aplicación autocontenida | Publicación con runtime incluido (`--self-contained true`) | Configurado en el script |
| Instalador | Paquete `.exe` de Inno Setup 6 (per-user, sin admin) | Configuración lista (`installer\rutati30.iss` + `scripts\build-installer.ps1`); compilar y probar en Windows |
| Prueba de instalación | Instalar en una VM limpia y arrancar | **Pendiente**: requiere Windows + Inno Setup 6 |

## 6. Generar el instalador (Inno Setup 6)

1. Instala [Inno Setup 6](https://jrsoftware.org/isinfo.php).
2. Ejecuta `scripts\publish.ps1` (el instalador empaqueta `artifacts\win-x64`).
3. Ejecuta:

```powershell
powershell -ExecutionPolicy Bypass -File scripts\build-installer.ps1
```

Salida: `artifacts\installer\rutati30-0.1.0-win-x64-setup.exe`.

Características del instalador (diseñadas, no probadas todavía):

- Instalación **por usuario** en `%LOCALAPPDATA%\Programs\RutaTI30`; no requiere administrador.
- Accesos directos en menú inicio y escritorio (opcional).
- **La desinstalación conserva tus datos** en `%LOCALAPPDATA%\RutaTI30`; bórralos manualmente solo si lo deseas (primero exporta un respaldo).
- El paquete no incluye credenciales ni datos personales.

> Esta configuración no ha sido compilada ni probada en Windows en el entorno actual de desarrollo; sigue pendiente de verificación en una VM limpia antes de distribuir.

## 7. Distribución sin instalador (portable)

1. Ejecuta `scripts\publish.ps1`.
2. Comprime `artifacts\win-x64`.
3. En el equipo destino, descomprime y ejecuta `RutaTI30.exe`.
4. Los datos se guardan siempre en el perfil del usuario; borrar la carpeta de la app no borra el progreso.

## 8. Actualización

1. Crea una copia de seguridad (Configuración → Crear copia de seguridad; ver BACKUP_RESTORE.md).
2. Reemplaza los archivos de la aplicación por la versión nueva (o instala el instalador cuando exista).
3. Al arrancar, las migraciones actualizan el esquema de la base de datos conservando el historial del usuario.

## 9. Desinstalación

Borra la carpeta de la aplicación. Los datos personales quedan en `%LOCALAPPDATA%\RutaTI30`; bórralos manualmente solo si el usuario lo desea (primero exporta un respaldo).
