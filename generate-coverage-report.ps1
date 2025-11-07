#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Generates code coverage report and opens it in Chrome browser
.DESCRIPTION
    Runs all tests with coverage collection, generates HTML report using ReportGenerator,
    and automatically opens it in Chrome browser
#>

param(
    [switch]$SkipBuild,
    [switch]$UnitTestsOnly,
    [switch]$IntegrationTestsOnly
)

Write-Host ""
Write-Host "===============================================================" -ForegroundColor Cyan
Write-Host "   NEBULA SPACE PARTS - CODE COVERAGE REPORT GENERATOR" -ForegroundColor Cyan
Write-Host "   'Precision is the bridge between survival and stardust.'" -ForegroundColor Gray
Write-Host "===============================================================" -ForegroundColor Cyan
Write-Host ""

# Clean previous coverage data
Write-Host "[1/6] ?? Cleaning previous coverage data..." -ForegroundColor Yellow
if (Test-Path "..\TestResults") {
    Remove-Item -Path "..\TestResults" -Recurse -Force
}
if (Test-Path "..\CoverageReport") {
    Remove-Item -Path "..\CoverageReport" -Recurse -Force
}
Write-Host "? Cleaned!" -ForegroundColor Green
Write-Host ""

# Build solution
if (-not $SkipBuild) {
    Write-Host "[2/6] ?? Building solution..." -ForegroundColor Yellow
    $buildResult = dotnet build webshopAPP.csproj --nologo --verbosity quiet 2>&1
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? Build failed!" -ForegroundColor Red
        Write-Host $buildResult -ForegroundColor Red
        exit 1
    }
    Write-Host "? Build successful!" -ForegroundColor Green
    Write-Host ""
}

# Run Unit Tests with coverage
if (-not $IntegrationTestsOnly) {
    Write-Host "[3/6] ?? Running Unit Tests with coverage collection..." -ForegroundColor Yellow
    Set-Location ..\UnitTests
    dotnet test --collect:"XPlat Code Coverage" --results-directory ..\TestResults\UnitTests --nologo --verbosity quiet
    Write-Host "? Unit tests completed!" -ForegroundColor Green
    Set-Location ..\webshopAI
    Write-Host ""
}

# Run Integration Tests with coverage
if (-not $UnitTestsOnly) {
    Write-Host "[4/6] ?? Running Integration Tests with coverage collection..." -ForegroundColor Yellow
    Set-Location ..\IntegrationTests
    dotnet test --collect:"XPlat Code Coverage" --results-directory ..\TestResults\IntegrationTests --nologo --verbosity quiet
    Write-Host "? Integration tests completed!" -ForegroundColor Green
    Set-Location ..\webshopAI
    Write-Host ""
}

# Generate HTML Report
Write-Host "[5/6] ?? Generating HTML coverage report..." -ForegroundColor Yellow
Set-Location ..
reportgenerator -reports:TestResults\**\coverage.cobertura.xml -targetdir:CoverageReport -reporttypes:Html

if ($LASTEXITCODE -ne 0) {
    Write-Host "? Report generation failed!" -ForegroundColor Red
    Write-Host "Make sure reportgenerator is installed: dotnet tool install -g dotnet-reportgenerator-globaltool" -ForegroundColor Yellow
    Set-Location webshopAI
    exit 1
}
Write-Host "? Report generated!" -ForegroundColor Green
Write-Host ""

# Open report in Chrome
Write-Host "[6/6] ?? Opening coverage report in Chrome browser..." -ForegroundColor Yellow
$reportPath = Join-Path (Get-Location) "CoverageReport\index.html"
Start-Process "chrome" $reportPath

Write-Host ""
Write-Host "===============================================================" -ForegroundColor Cyan
Write-Host "           COVERAGE REPORT OPENED IN CHROME!" -ForegroundColor Green
Write-Host "===============================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "?? Report location: $reportPath" -ForegroundColor Gray
Write-Host ""
Write-Host "The report shows:" -ForegroundColor White
Write-Host "  ? Overall coverage percentage" -ForegroundColor Gray
Write-Host "  ? Line coverage per file" -ForegroundColor Gray
Write-Host "  ? Branch coverage" -ForegroundColor Gray
Write-Host "  ? Color-coded: Green (covered), Red (not covered)" -ForegroundColor Gray
Write-Host ""

Set-Location webshopAI
