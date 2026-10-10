; ============================================================
; Ruta TI 30 - Script de instalador (Inno Setup 6)
; ============================================================
; Compilar con ISCC.exe (ver scripts/build-installer.ps1).
; Requisitos de compilación:
;   1. Haber ejecutado scripts\publish.ps1 (genera artifacts\win-x64).
;   2. Inno Setup 6 instalado (https://jrsoftware.org/isinfo.php).
;
; Decisiones de diseño:
;   - Instalación POR USUARIO (PrivilegesRequired=lowest): la app no
;     requiere privilegios de administrador para funcionar.
;   - Los datos del usuario viven en {localappdata}\RutaTI30 y NUNCA se
;     eliminan en la desinstalación (el usuario los borra manualmente
;     si lo desea; ver BACKUP_RESTORE.md).
;   - No se incluyen credenciales ni información personal en el paquete.
; ============================================================

#define AppName "Ruta TI 30"
#define AppVersion "0.1.0"
#define AppPublisher "Ruta TI 30"
#define AppExeName "RutaTI30.exe"
#define SourceDir "..\artifacts\win-x64"

[Setup]
AppId={{8A6F3C21-5E4B-4C7D-9A10-RUTATI300001}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={localappdata}\Programs\RutaTI30
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
OutputDir=..\artifacts\installer
OutputBaseFilename=rutati30-{#AppVersion}-win-x64-setup
; Nota: no se define SetupIconFile porque el repositorio no incluye un archivo .ico;
; el instalador usa el icono por defecto de Inno Setup. Si se añade
; src\RutaTI30.App\app.ico, descomentar la siguiente línea:
; SetupIconFile=..\src\RutaTI30.App\app.ico
UninstallDisplayIcon={app}\{#AppExeName}
Compression=lzma2/max
SolidCompression=yes
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
WizardStyle=modern
CloseApplications=yes
; La app guarda datos en el perfil del usuario: la desinstalación no toca
; {localappdata}\RutaTI30 (decisión documentada en BACKUP_RESTORE.md).

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
; Requiere haber ejecutado scripts\publish.ps1 antes de compilar este script.
Source: "{#SourceDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExeName}"; Description: "{cm:LaunchProgram,{#AppName}}"; Flags: nowait postinstall skipifsilent

[UninstallRun]
; No se ejecutan comandos de limpieza: los datos del usuario se conservan
; a propósito. El mensaje de desinstalación lo indica al usuario.

[Messages]
; Nota honesta mostrada al desinstalar.
