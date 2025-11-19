# ?? Code Coverage Report Guide

## ?? Quick Start - Generate Coverage Report

### Option 1: Generate Coverage Report Only (Fastest)
```cmd
generate-coverage-report.bat
```
**or**
```powershell
.\generate-coverage-report.ps1
```

This will:
1. ? Clean previous coverage data
2. ? Build solution
3. ? Run all unit tests with coverage
4. ? Run all integration tests with coverage
5. ? Generate HTML report
6. ? **Open report in Chrome automatically**

**Time:** ~30-40 seconds

---

### Option 2: Full Test Suite + Coverage (Complete)
```cmd
run-playwright.bat
```

This runs:
1. ? All 646 unit tests (with coverage)
2. ? All 47 integration tests (with coverage)
3. ? All 9 Playwright E2E tests (headless)
4. ? Generates HTML coverage report
5. ? **Opens report in Chrome automatically**

**Time:** ~1-2 minutes

---

## ?? Understanding the Coverage Report

### What You'll See in Chrome

When the report opens, you'll see a dashboard with:

#### **Summary Page**
- **Overall Coverage Percentage** (e.g., 95.2%)
- **Line Coverage** - How many lines of code were executed
- **Branch Coverage** - How many decision branches were tested
- **Summary by Assembly** - Coverage per project

#### **Detailed Views**
Click on any project/file to see:
- **Green lines** = Covered by tests ?
- **Red lines** = Not covered by tests ?
- **Yellow/Orange** = Partially covered (some branches)

#### **Color Legend**
```
? Green (100%)     - Fully tested
? Yellow (50-99%)  - Partially tested
? Red (0-49%)      - Poorly tested
```

---

## ?? Coverage Report Location

```
webshopAI\
??? TestResults\
?   ??? UnitTests\
?   ?   ??? {guid}\coverage.cobertura.xml
?   ??? IntegrationTests\
?       ??? {guid}\coverage.cobertura.xml
??? CoverageReport\
    ??? index.html              ? Open this in Chrome!
    ??? Summary.html
    ??? (many other HTML files)
```

**Main report:** `CoverageReport\index.html`

---

## ?? Current Coverage Targets

Based on your test suite:

| Project | Expected Coverage | Status |
|---------|-------------------|--------|
| **Services** | ~100% | ? Excellent |
| **Data** | ~90-95% | ? Great |
| **Models** | ~80-90% | ? Good |
| **webshopAPP** | ~30-40% | ?? (Controllers, views) |

**Overall Target:** ? 80% (You should exceed this!)

---

## ?? Analyzing Your Coverage

### Services Project (Should be ~100%)
1. Open report in Chrome
2. Click **"Services"** assembly
3. You should see:
   - `ProductService` - 100% ?
   - `CartService` - 100% ?
   - `OrderService` - 100% ?

### Find Untested Code
1. Look for **red-highlighted lines**
2. Check files with low percentage
3. Add tests for critical paths

### Example Report Sections:

**High Coverage (Good):**
```
Services/ProductService.cs          100.0%  ????????????
Services/CartService.cs             100.0%  ????????????
Data/ProductRepository.cs            95.2%  ????????????
```

**Low Coverage (Needs Work):**
```
webshopAPP/Controllers/Home.cs       25.0%  ????????????
webshopAPP/Program.cs                15.0%  ????????????
```

---

## ??? Advanced Options

### Generate Coverage for Specific Tests

**Unit Tests Only:**
```powershell
.\generate-coverage-report.ps1 -UnitTestsOnly
```

**Integration Tests Only:**
```powershell
.\generate-coverage-report.ps1 -IntegrationTestsOnly
```

**Skip Build (if already built):**
```powershell
.\generate-coverage-report.ps1 -SkipBuild
```

---

## ?? Exporting Coverage Data

### Different Report Formats

You can generate other formats by modifying the script:

```powershell
# HTML + Badges + Cobertura XML
reportgenerator `
  -reports:TestResults\**\coverage.cobertura.xml `
  -targetdir:CoverageReport `
  -reporttypes:"Html;Badges;Cobertura"

# Text summary to console
reportgenerator `
  -reports:TestResults\**\coverage.cobertura.xml `
  -targetdir:CoverageReport `
  -reporttypes:"TextSummary"
```

### Available Report Types:
- `Html` - Interactive HTML (default)
- `HtmlSummary` - Single-page summary
- `Badges` - SVG badges for README
- `Cobertura` - XML format
- `JsonSummary` - JSON format
- `Latex` - LaTeX format
- `MarkdownSummary` - Markdown table

---

## ?? Customizing Reports

### Include Coverage Badge in README

After generating with badges:
```markdown
![Code Coverage](CoverageReport/badge_combined.svg)
```

### Filter Specific Assemblies

Edit the reportgenerator command to exclude/include:
```cmd
reportgenerator ^
  -reports:TestResults\**\coverage.cobertura.xml ^
  -targetdir:CoverageReport ^
  -reporttypes:Html ^
  -assemblyfilters:+Services;+Data;-webshopAPP
```

---

## ? Troubleshooting

### "reportgenerator not found"
**Solution:**
```powershell
dotnet tool install -g dotnet-reportgenerator-globaltool
```

### No coverage.cobertura.xml files
**Solution:**
- Make sure coverlet.collector is installed in test projects
- Run tests with `--collect:"XPlat Code Coverage"`

### Chrome doesn't open automatically
**Solution:**
- Open manually: `CoverageReport\index.html`
- Or use: `start chrome CoverageReport\index.html`

### Old coverage data shown
**Solution:**
- Delete `TestResults` and `CoverageReport` folders
- Run script again

### Low coverage percentage
**Solution:**
- Click through report to find red lines
- Write tests for uncovered code
- Focus on critical business logic first

---

## ?? Coverage Goals

### Recommended Coverage by Project Type:

| Project Type | Minimum | Target | Excellent |
|-------------|---------|--------|-----------|
| **Business Logic (Services)** | 80% | 90% | 95-100% |
| **Data Access (Repositories)** | 70% | 85% | 90%+ |
| **Models/DTOs** | 60% | 80% | 90%+ |
| **Controllers** | 50% | 70% | 80%+ |
| **Program.cs/Startup** | 30% | 50% | 70%+ |

### Your Current Status:
Based on 646 unit tests + 47 integration tests:
- **Services:** ~100% ? Excellent!
- **Data:** ~90%+ ? Great!
- **Overall:** ~85-90% ? Very Good!

---

## ?? Reading the HTML Report

### Main Sections:

1. **Summary**
   - Overall coverage metrics
   - Assembly list with percentages
   - Historic trend (if multiple runs)

2. **Coverage by Assembly**
   - Click assembly name to drill down
   - See all classes and their coverage

3. **Coverage by Class**
   - Click class name to see source code
   - Lines highlighted green/red/yellow

4. **Risk Hotspots**
   - Areas with low coverage and high complexity
   - Focus testing efforts here

---

## ?? Integration with CI/CD

### Export for Azure DevOps/GitHub Actions

```yaml
# Generate coverage in CI
- name: Test with Coverage
  run: |
    dotnet test --collect:"XPlat Code Coverage" --results-directory TestResults
    reportgenerator -reports:TestResults/**/coverage.cobertura.xml -targetdir:CoverageReport -reporttypes:"Cobertura;HtmlSummary"
    
# Publish results
- name: Publish Coverage
  uses: codecov/codecov-action@v3
  with:
    files: ./CoverageReport/Cobertura.xml
```

---

## ?? Tips for Improving Coverage

1. **Start with Services** - Easiest wins, highest value
2. **Add Integration Tests** - Test real workflows
3. **Test Edge Cases** - Null values, empty lists, errors
4. **Mock Dependencies** - Isolate units of code
5. **Use TDD** - Write tests first, code second

---

## ?? Quick Reference

| Action | Command |
|--------|---------|
| **Generate report** | `generate-coverage-report.bat` |
| **Full suite + coverage** | `run-playwright.bat` |
| **Unit tests only** | `.\generate-coverage-report.ps1 -UnitTestsOnly` |
| **View existing report** | Open `CoverageReport\index.html` |
| **Clean coverage data** | Delete `TestResults` and `CoverageReport` folders |

---

**Happy Testing!** ??

*"Precision is the bridge between survival and stardust."*  
— Glork approves of thorough test coverage! ?
