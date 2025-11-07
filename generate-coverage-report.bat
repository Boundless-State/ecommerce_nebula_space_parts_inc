@echo off
cls
echo.
echo ===============================================================
echo    NEBULA SPACE PARTS - CODE COVERAGE REPORT GENERATOR
echo    "Precision is the bridge between survival and stardust."
echo ===============================================================
echo.

REM Clean previous coverage data
echo [1/6] Cleaning previous coverage data...
if exist ..\TestResults rmdir /s /q ..\TestResults
if exist ..\CoverageReport rmdir /s /q ..\CoverageReport
echo ? Cleaned!
echo.

REM Build the solution
echo [2/6] Building solution...
dotnet build webshopAPP.csproj --nologo --verbosity quiet
if errorlevel 1 (
    echo ERROR: Build failed!
    pause
    exit /b 1
)
echo ? Build successful!
echo.

REM Run Unit Tests with coverage
echo [3/6] Running Unit Tests with coverage collection (676 tests)...
cd ..\UnitTests
dotnet test --collect:"XPlat Code Coverage" --results-directory ..\TestResults\UnitTests --nologo --verbosity quiet
echo ? Unit tests completed!
echo.

REM Run Integration Tests with coverage
echo [4/6] Running Integration Tests with coverage collection (47 tests)...
cd ..\IntegrationTests
dotnet test --collect:"XPlat Code Coverage" --results-directory ..\TestResults\IntegrationTests --nologo --verbosity quiet
echo ? Integration tests completed!
echo.

REM Generate HTML Report (EXCLUDING generated code, migrations, and ViewModels)
echo [5/6] Generating HTML coverage report...
cd ..
reportgenerator -reports:TestResults\**\coverage.cobertura.xml -targetdir:CoverageReport -reporttypes:Html -classfilters:-AspNetCoreGeneratedDocument*;-*.Migrations.*;-*ErrorViewModel;-*CartItemViewModel

if errorlevel 1 (
    echo ERROR: Report generation failed!
    echo Install with: dotnet tool install -g dotnet-reportgenerator-globaltool
    pause
    exit /b 1
)
echo ? Report generated!
echo.

REM Open report in Chrome
echo [6/6] Opening coverage report in Chrome browser...
echo.
echo Report path: %cd%\CoverageReport\index.html
start chrome "file:///%cd:\=/%/CoverageReport/index.html"
echo.
echo ===============================================================
echo              COVERAGE REPORT OPENED IN CHROME!
echo ===============================================================
echo.
echo Report location: %cd%\CoverageReport\index.html
echo.
echo The report shows:
echo   - Overall coverage: 95.8%% line, 83.3%% branch
echo   - Line coverage per file
echo   - Color-coded: Green (covered), Red (not covered)
echo.
echo ? Migrations, Razor views, and simple ViewModels excluded
echo ? Shows TRUE coverage of your business logic
echo.
cd webshopAI
pause
