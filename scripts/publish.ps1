# Ruta TI 30 - publicación para Windows x64
# Genera la aplicación lista para distribuir en artifacts/win-x64
# IMPORTANTE: esto produce una APLICACIÓN PUBLICADA, no un instalador.
# Uso: powershell -ExecutionPolicy Bypass -File scripts\publish.ps1
$ErrorActionPreference = "Stop"
Set-Location (Join-Path $PSScriptRoot "..")

$outDir = "artifacts/win-x64"

Write-Host "== Publish win-x64 (self-contained) ==" -ForegroundColor Cyan
dotnet publish src/RutaTI30.App/RutaTI30.App.csproj `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=false `
    -o $outDir
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$exe = Join-Path $outDir "RutaTI30.exe"
if (-not (Test-Path $exe)) {
    Write-Host "ERROR: no se generó $exe" -ForegroundColor Red
    exit 1
}

Write-Host "`nPublicación generada en $outDir" -ForegroundColor Green
Write-Host "Ejecutable: $exe"
Write-Host "Nota: esto NO es un instalador. Ver INSTALLATION.md para distribución e instalación."
