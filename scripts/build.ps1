# Ruta TI 30 - compilación Debug y Release
# Requisitos: Windows 10/11 con .NET 10 SDK instalado.
# Uso: powershell -ExecutionPolicy Bypass -File scripts\build.ps1
$ErrorActionPreference = "Stop"
Set-Location (Join-Path $PSScriptRoot "..")

Write-Host "== dotnet SDK ==" -ForegroundColor Cyan
dotnet --info

Write-Host "`n== Restore ==" -ForegroundColor Cyan
dotnet restore RutaTI30.sln
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "`n== Build Debug ==" -ForegroundColor Cyan
dotnet build RutaTI30.sln -c Debug --no-restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "`n== Build Release ==" -ForegroundColor Cyan
dotnet build RutaTI30.sln -c Release --no-restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "`nCompilación Debug y Release completada." -ForegroundColor Green
