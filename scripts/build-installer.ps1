# Ruta TI 30 - generación del instalador con Inno Setup 6
# Precondición: haber ejecutado scripts\publish.ps1 (genera artifacts\win-x64).
# Requisitos: Windows 10/11 x64, .NET 10 SDK e Inno Setup 6 (ISCC.exe) instalados.
# Uso: powershell -ExecutionPolicy Bypass -File scripts\build-installer.ps1
$ErrorActionPreference = "Stop"
Set-Location (Join-Path $PSScriptRoot "..")

Write-Host "== Verificar artefacto publicado ==" -ForegroundColor Cyan
$pubDir = "artifacts\win-x64"
if (-not (Test-Path (Join-Path $pubDir "RutaTI30.exe"))) {
    Write-Host "No se encontró artifacts\win-x64\RutaTI30.exe." -ForegroundColor Red
    Write-Host "Ejecuta primero: powershell -ExecutionPolicy Bypass -File scripts\publish.ps1" -ForegroundColor Yellow
    exit 1
}

Write-Host "`n== Buscar ISCC.exe (Inno Setup 6) ==" -ForegroundColor Cyan
$iscc = Get-Command "ISCC.exe" -ErrorAction SilentlyContinue
if (-not $iscc) {
    $candidatos = @(
        "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
        "$env:ProgramFiles\Inno Setup 6\ISCC.exe"
    )
    foreach ($c in $candidatos) {
        if (Test-Path $c) { $iscc = $c; break }
    }
}
if (-not $iscc) {
    Write-Host "ISCC.exe no encontrado. Instala Inno Setup 6 desde https://jrsoftware.org/isinfo.php" -ForegroundColor Red
    Write-Host "y vuelve a ejecutar este script." -ForegroundColor Yellow
    exit 1
}
Write-Host "ISCC: $iscc"

Write-Host "`n== Compilar instalador ==" -ForegroundColor Cyan
& $iscc "installer\rutati30.iss"
if ($LASTEXITCODE -ne 0) {
    Write-Host "Compilación del instalador FALLIDA (código $LASTEXITCODE)." -ForegroundColor Red
    exit $LASTEXITCODE
}

$setup = "artifacts\installer\rutati30-0.1.0-win-x64-setup.exe"
if (-not (Test-Path $setup)) {
    Write-Host "ERROR: no se generó $setup" -ForegroundColor Red
    exit 1
}

Write-Host "`nInstalador generado en $setup" -ForegroundColor Green
Write-Host "Verifícalo en una VM limpia antes de distribuir (ver INSTALLATION.md)."
Write-Host "La instalación es por usuario (sin administrador) y la desinstalación conserva tus datos en %LOCALAPPDATA%\RutaTI30."
