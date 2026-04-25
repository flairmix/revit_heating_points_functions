$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$solution = Join-Path $scriptDir "Fill_ADSK_Parameters.sln"
$projectDir = Join-Path $scriptDir "Fill_ADSK_Parameters"
$dllPath = Join-Path $projectDir "bin\Release\Fill_ADSK_Parameters.dll"
$manifestPath = Join-Path $projectDir "Fill_ADSK_Parameters_2025.addin"

Write-Host "Building Fill_ADSK_Parameters for Revit 2025..."
dotnet build $solution -c Release

Write-Host ""
Write-Host "Build completed."
Write-Host "DLL: $dllPath"
Write-Host "Manifest: $manifestPath"
