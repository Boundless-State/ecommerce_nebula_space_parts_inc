# Code Coverage Report

## Overview
This project uses **Coverlet** for code coverage collection and reporting.

## Current Coverage
- **Line Coverage:** 14.91% (300/2011 lines covered)
- **Branch Coverage:** 19.29% (22/114 branches covered)

## Running Tests with Coverage

### Unit Tests with Coverage
```bash
dotnet test ../UnitTests/UnitTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=../TestResults/
```

### Integration Tests with Coverage
```bash
dotnet test ../IntegrationTests/IntegrationTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=../TestResults/integration-
```

### All Tests with Coverage (Combined Report)
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=../TestResults/coverage.cobertura.xml
```

## Coverage Report Formats

Coverlet supports multiple output formats:
- `cobertura` - XML format compatible with most CI/CD tools
- `lcov` - Compatible with lcov tools
- `opencover` - XML format for ReportGenerator
- `json` - JSON format
- `html` - Direct HTML output (requires ReportGenerator)

### Generate HTML Report
To generate an HTML coverage report, install ReportGenerator and run:

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:../TestResults/coverage.cobertura.xml -targetdir:../TestResults/CoverageReport -reporttypes:Html
```

Then open `../TestResults/CoverageReport/index.html` in your browser.

## Coverage Configuration

Coverage settings are configured via MSBuild properties:
- **Include**: Assemblies to include in coverage (Models, Data, Services, webshopAPP)
- **Exclude**: Assemblies to exclude (Tests, Migrations, Program)
- **ExcludeByAttribute**: Attributes that mark code to exclude
- **ExcludeByFile**: File patterns to exclude

## Continuous Integration

Add this to your CI pipeline (GitHub Actions, Azure DevOps, etc.):

```yaml
- name: Run tests with coverage
  run: dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
  
- name: Generate coverage report
  run: reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage-report -reporttypes:Html;Badges
  
- name: Upload coverage
  uses: codecov/codecov-action@v3
  with:
    files: ./TestResults/coverage.cobertura.xml
```

## Test Statistics
- **Total Tests:** 519
  - Unit Tests: 512
  - Integration Tests: 7
- **Pass Rate:** 100%
- **AAA Pattern:** All tests follow Arrange-Act-Assert
- **Single Assertion:** Each test has exactly one assertion

## Improving Coverage

To improve coverage, focus on:
1. **Controllers** - Add more controller action tests
2. **ViewComponents** - Test cart icon and other view components
3. **Edge Cases** - Add boundary and error condition tests
4. **Integration Paths** - More end-to-end scenarios

## Coverage Thresholds

To enforce minimum coverage in CI:

```bash
dotnet test /p:CollectCoverage=true /p:Threshold=80 /p:ThresholdType=line
```

This will fail the build if line coverage drops below 80%.
