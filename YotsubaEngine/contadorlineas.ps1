# Obtiene la carpeta donde está ubicado este script
$carpetaActual = Split-Path -Parent $MyInvocation.MyCommand.Path

# Busca todos los archivos .cs en la carpeta actual y subcarpetas
$archivosCS = Get-ChildItem -Path $carpetaActual -Recurse -Filter *.cs -File

$totalLineas = 0

foreach ($archivo in $archivosCS) {
    $lineas = (Get-Content $archivo.FullName).Count
    $totalLineas += $lineas
}

Write-Host "======================================"
Write-Host " Conteo de lineas de codigo C#"
Write-Host "======================================"
Write-Host "Carpeta analizada: $carpetaActual"
Write-Host "Archivos .cs encontrados: $($archivosCS.Count)"
Write-Host "Total de lineas: $totalLineas"
Write-Host "======================================"

Write-Host ""
Write-Host "Presiona cualquier tecla para salir..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
