# Ruta TI 30 - pruebas automatizadas (xUnit)
# Uso: powershell -ExecutionPolicy Bypass -File scripts\test.ps1
$ErrorActionPreference = "Stop"
Set-Location (Join-Path $PSScriptRoot "..")

Write-Host "== dotnet test (Release) ==" -ForegroundColor Cyan
dotnet test RutaTI30.sln -c Release --logger "console;verbosity=normal"
if ($LASTEXITCODE -ne 0) {
    Write-Host "`nPruebas FALLIDAS (código $LASTEXITCODE)." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "`nTodas las pruebas aprobaron." -ForegroundColor Green
