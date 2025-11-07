# ?? Nebula Space Parts - AI-Assisted Webshop

> *"If it's from Glork... you can trust it to bring you home."*

[![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4)](https://dotnet.microsoft.com/apps/aspnet)
[![Tests](https://img.shields.io/badge/Tests-732%20passing-success)](./UnitTests)
[![Coverage](https://img.shields.io/badge/Coverage-95.8%25-brightgreen)](./COVERAGE-GUIDE.md)
[![AI Assisted](https://img.shields.io/badge/AI-Copilot%20%7C%20GPT--4%20%7C%20Claude-blue)](https://github.com/features/copilot)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

A fully functional, AI-assisted e-commerce platform for intergalactic space parts, built with ASP.NET Core MVC, featuring comprehensive test coverage (95.8%), automated testing with Playwright, and professional code quality standards.

**Founded by The Big Glork in 2261** to provide reliable quantum hyperdrives, flux capacitors, and other essential spacecraft components to explorers across the cosmos.

**Built with AI assistance:** This project was developed using GitHub Copilot, ChatGPT-4, and Claude Sonnet 3.5 to accelerate development, improve code quality, and ensure comprehensive test coverage.

---

## ?? Table of Contents

- [Features](#-features)
- [Tech Stack](#-tech-stack)
- [AI Development Tools](#-ai-development-tools)
- [Testing Tools & Coverage](#-testing-tools--coverage)
- [Project Structure](#-project-structure)
- [Getting Started](#-getting-started)
- [Running the Application](#-running-the-application)
- [Testing](#-testing)
- [Code Coverage](#-code-coverage)
- [Playwright E2E Tests](#-playwright-e2e-tests)
- [Generating Reports](#-generating-reports)
- [Development](#-development)
- [Contributing](#-contributing)
- [The Glork Guarantee](#-the-glork-guarantee)

---

## ? Features

### ??? E-Commerce Functionality
- **Product Catalog** - Browse quantum hyperdrives, flux capacitors, and more
- **Advanced Search** - Find parts by name, description, or category
- **Shopping Cart** - Session-based cart with real-time updates
- **Checkout System** - Complete order processing with payment simulation
- **Admin Panel** - CRUD operations for product management

### ?? User Experience
- **Responsive Design** - Bootstrap-powered, works on all devices
- **Themed Interface** - Custom space-themed UI with background images
  - Spaceport Station background (Home & About pages)
  - Warehouse background (Products & Cart pages)
- **Cart Icon** - Real-time item count in navigation bar
- **Product Details** - Detailed view with images and specifications

### ??? Quality & Testing
- **95.8% Code Coverage** - Comprehensive unit and integration tests
- **83.3% Branch Coverage** - Edge cases and error paths tested
- **732 Total Tests** - 676 unit + 47 integration + 9 E2E tests
- **Automated Testing** - One-click test execution with coverage
- **Playwright E2E** - Headless browser automation tests
- **CI/CD Ready** - Automated test suite with coverage reports

---

## ??? Tech Stack

### **Backend**
- **.NET 9** - Latest .NET framework
- **ASP.NET Core MVC** - Model-View-Controller architecture
- **Entity Framework Core** - ORM for database operations
- **SQLite** - Lightweight database (dev) / SQL Server (production)

### **Frontend**
- **Razor Views** - Server-side rendering
- **Bootstrap 5** - Responsive CSS framework
- **Custom CSS** - Space-themed styling

### **Testing**
- **xUnit** - Unit testing framework
- **Moq** - Mocking framework for dependencies
- **Coverlet** - Code coverage collection
- **ReportGenerator** - HTML coverage report generation
- **Playwright** - E2E browser automation (headless Chrome)

### **Tools & Utilities**
- **FluentValidation** (optional) - Model validation
- **NuGet** - Package management
- **Git** - Version control

---

## ?? AI Development Tools

This project was built with assistance from cutting-edge AI tools to accelerate development, improve code quality, and ensure comprehensive testing:

### **GitHub Copilot**
- **Usage:** Real-time code suggestions, auto-completion, test generation
- **Impact:** 40% faster development, fewer bugs, consistent code patterns
- **Best for:** Writing boilerplate code, generating tests, refactoring

### **ChatGPT-4 (OpenAI)**
- **Usage:** Architecture decisions, code reviews, documentation generation
- **Impact:** Better design patterns, improved documentation quality
- **Best for:** Complex problem-solving, explaining code, generating docs

### **Claude Sonnet 3.5 (Anthropic)**
- **Usage:** Code analysis, test strategy, comprehensive testing scenarios
- **Impact:** 95.8% test coverage, edge case identification, negative scenario testing
- **Best for:** Deep code analysis, test coverage improvement, refactoring suggestions

### **AI-Assisted Development Stats**
```
Total Lines of Code:     ~5,000
AI-Generated Code:       ~30%
AI-Assisted Tests:       ~50%
Test Coverage:           95.8%
Branch Coverage:         83.3%
Tests Written:           732
Time Saved:              ~40 hours
```

---

## ?? Testing Tools & Coverage

### **Testing Framework Stack**

#### **1. xUnit - Unit Testing Framework**
```bash
# Install
dotnet add package xunit

# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "FullyQualifiedName~CartServiceTests"
```

**What it does:** Runs unit and integration tests with Assert statements

---

#### **2. Moq - Mocking Framework**
```bash
# Install
dotnet add package Moq

# Example usage in tests
var mockService = new Mock<IProductService>();
mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(product);
```

**What it does:** Creates fake objects for dependency injection in tests

---

#### **3. Coverlet - Code Coverage Collection**
```bash
# Install
dotnet add package coverlet.collector

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Coverage files saved to: TestResults/{guid}/coverage.cobertura.xml
```

**What it does:** Collects coverage data during test execution

---

#### **4. ReportGenerator - HTML Coverage Reports**
```bash
# Install globally
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate HTML report from coverage files
reportgenerator \
  -reports:TestResults/**/coverage.cobertura.xml \
  -targetdir:CoverageReport \
  -reporttypes:Html

# Open report
start chrome CoverageReport/index.html
```

**What it does:** Converts coverage.cobertura.xml to interactive HTML reports

---

#### **5. Playwright - E2E Browser Automation**
```bash
# Install
dotnet add package Microsoft.Playwright

# Install browsers
pwsh bin/Debug/net9.0/playwright.ps1 install

# Run E2E tests
dotnet test --filter "PlaywrightE2ETests"
```

**What it does:** Automates browser testing (Chrome, Firefox, Edge) in headless mode

---

## ?? Project Structure

```
webshopAI/
??? webshopAI/                    # Main MVC application
?   ??? Controllers/              # MVC Controllers
?   ??? Views/                    # Razor Views
?   ??? wwwroot/                  # Static files (CSS, images)
?   ??? Program.cs                # Application entry point
?   ??? appsettings.json          # Configuration
?
??? Services/                     # Business logic layer
?   ??? Implementations/
?   ?   ??? CartService.cs        # Shopping cart logic (100% coverage)
?   ?   ??? ProductService.cs     # Product management (100% coverage)
?   ?   ??? MockPaymentProvider.cs
?   ??? Interfaces/               # Service interfaces
?
??? Data/                         # Data access layer
?   ??? WebshopDbContext.cs       # EF Core context (100% coverage)
?   ??? Migrations/               # Database migrations (excluded from coverage)
?
??? Models/                       # Domain models & ViewModels
?   ??? Entities/                 # Database entities (100% coverage)
?   ??? ViewModels/               # View models (95.8% coverage)
?
??? UnitTests/                    # Unit tests (676 tests)
?   ??? CartServiceTests.cs
?   ??? ProductServiceTests.cs
?   ??? NegativeScenarios_Tests.cs  # Dedicated negative test file
?   ??? EdgeCase_BranchCoverage_Tests.cs
?   ??? PlaywrightE2ETests.cs    # E2E tests (9 tests)
?
??? IntegrationTests/             # Integration tests (47 tests)
?   ??? API/                      # API endpoint tests
?
??? TestResults/                  # Coverage data (generated)
??? CoverageReport/               # HTML coverage report (generated)
?
??? run-playwright.bat            # Run full test suite (Windows)
??? run-playwright-tests.ps1      # Run full test suite (PowerShell)
??? generate-coverage-report.bat  # Generate coverage only
??? README.md                     # This file
```

---

## ?? Getting Started

### Prerequisites

- **.NET 9 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Git** - [Download here](https://git-scm.com/downloads)
- **Visual Studio 2022** or **VS Code** (recommended)
- **Chrome/Chromium** - For Playwright E2E tests

### Optional Tools
- **ReportGenerator** - For coverage reports (install below)
- **Playwright browsers** - Auto-installed by tests

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/Boundless-State/webshopAI.git
cd webshopAI/webshopAI
```

2. **Restore dependencies**
```bash
dotnet restore
```

3. **Build the solution**
```bash
dotnet build
```

4. **Initialize the database**
```bash
dotnet ef database update
```
The database will auto-seed with 10 demo products on first run.

5. **Install ReportGenerator (for coverage reports)**
```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```

6. **Install Playwright browsers (for E2E tests)**
```bash
pwsh bin/Debug/net9.0/playwright.ps1 install
```
Or let the tests install it automatically on first run.

---

## ?? Running the Application

### **Option 1: Visual Studio**
1. Open `webshopAI.sln`
2. Set `webshopAI` as startup project
3. Press `F5` or click **Run**
4. Navigate to `https://localhost:5001`

### **Option 2: Command Line**
```bash
cd webshopAI
dotnet run
```
Then open browser to: `https://localhost:5001`

### **Option 3: Watch Mode (Auto-reload)**
```bash
dotnet watch run
```
Changes to code will automatically reload the app.

### **First Run**
On first run:
- ? Database created automatically
- ? 10 demo products seeded
- ? Categories created (Quantum Parts, Navigation, Power Systems, etc.)
- ? Sample images configured

---

## ?? Testing

### **Quick Test Suite (Recommended)**

**Windows:**
```cmd
run-playwright.bat
```

**PowerShell/Linux/Mac:**
```powershell
./run-playwright-tests.ps1
```

This runs:
1. ? **Builds** the solution
2. ? **Starts** the web server (for Playwright tests)
3. ? **Runs 676 unit tests** with coverage
4. ? **Runs 47 integration tests** with coverage
5. ? **Runs 9 Playwright E2E tests** (headless mode)
6. ? **Generates HTML coverage report**
7. ? **Opens report in Chrome**

**Total time:** ~1-2 minutes

---

### **Individual Test Suites**

#### **Unit Tests Only (676 tests)**
```bash
cd UnitTests
dotnet test
```

**What's tested:**
- CartService (100% coverage)
- ProductService (100% coverage)
- MockPaymentProvider (100% coverage)
- Edge cases and boundary conditions
- Negative scenarios (dedicated file)
- Model validation
- Business logic

#### **Integration Tests Only (47 tests)**
```bash
cd IntegrationTests
dotnet test
```

**What's tested:**
- API endpoints
- Database operations
- Controller actions
- Full workflows
- HTTP contracts

#### **Playwright E2E Tests Only (9 tests)**
```bash
cd UnitTests
dotnet test --filter "PlaywrightE2ETests"
```

**What's tested:**
- Home page navigation
- Product search and filtering
- Product details view
- Add to cart functionality
- Cart page verification
- Complete user journey (search ? details ? cart ? checkout)
- Navigation links

**Note:** Requires the app to be running at `https://localhost:5001`

---

## ?? Code Coverage

### **Generate Coverage Report**

**Quick method (recommended):**
```cmd
generate-coverage-report.bat
```

This will:
1. Clean previous coverage data
2. Build solution
3. Run all 676 unit tests with coverage
4. Run all 47 integration tests with coverage
5. Generate interactive HTML report
6. Open report in Chrome browser

**Report location:** `CoverageReport/index.html`

---

### **Manual Coverage Generation (Step-by-Step)**

#### **Step 1: Run Tests with Coverage**
```bash
# Unit tests
cd UnitTests
dotnet test --collect:"XPlat Code Coverage" --results-directory ../TestResults/UnitTests

# Integration tests
cd ../IntegrationTests
dotnet test --collect:"XPlat Code Coverage" --results-directory ../TestResults/IntegrationTests
```

#### **Step 2: Generate HTML Report**
```bash
cd ..
reportgenerator \
  -reports:TestResults/**/coverage.cobertura.xml \
  -targetdir:CoverageReport \
  -reporttypes:Html \
  -classfilters:-AspNetCoreGeneratedDocument*;-*.Migrations.*;-*ErrorViewModel;-*CartItemViewModel
```

**Exclusion Filters:**
- `-AspNetCoreGeneratedDocument*` - Excludes Razor compiled views
- `-*.Migrations.*` - Excludes EF Core migrations
- `-*ErrorViewModel` - Excludes simple error DTO
- `-*CartItemViewModel` - Excludes simple cart DTO

#### **Step 3: Open Report**
```bash
# Windows
start chrome CoverageReport/index.html

# Linux/Mac
open CoverageReport/index.html
```

---

### **Coverage Statistics**

```
Overall Coverage:  95.8% ?
Branch Coverage:   83.3% ?
Method Coverage:   96.0% ?

By Assembly:
?? Services:      100.0% ????????????  PERFECT!
?? Data:          100.0% ????????????  PERFECT!
?? Models:         95.8% ????????????  EXCELLENT!
?? Controllers:    89.0% ????????????  VERY GOOD!
```

### **What's Excluded from Coverage**
To show true business logic coverage, these are excluded:
- ? ASP.NET Core generated Razor views (`AspNetCoreGeneratedDocument*`)
- ? EF Core database migrations (`*.Migrations.*`)
- ? Auto-generated `.cshtml.g.cs` files
- ? Simple DTOs (`ErrorViewModel`, `CartItemViewModel`)

### **Using the Coverage Report**

1. **Open the report** (auto-opens in Chrome)
2. **Click on an assembly** (e.g., "Services") to drill down
3. **Click on a class** to see source code
4. **View line-by-line coverage:**
   - ?? **Green** = Covered by tests
   - ?? **Red** = Not covered
   - ?? **Yellow** = Partially covered (some branches)

### **Coverage Goals**
| Component | Target | Actual | Status |
|-----------|--------|--------|--------|
| **Services** | ?90% | 100.0% | ? Exceeded |
| **Data** | ?85% | 100.0% | ? Exceeded |
| **Models** | ?80% | 95.8% | ? Exceeded |
| **Controllers** | ?70% | 89.0% | ? Exceeded |
| **Overall** | ?80% | 95.8% | ? Exceeded |

---

## ?? Playwright E2E Tests

### **What is Playwright?**
Playwright is a browser automation framework that simulates real user interactions. Our tests run in **headless mode** (no visible browser) for speed and CI/CD compatibility.

### **Running Playwright Tests**

#### **Method 1: Included in Full Test Suite**
```bash
# Runs everything including Playwright
run-playwright.bat
```

#### **Method 2: Playwright Only**
```bash
# Start app first
cd webshopAI
dotnet run

# In another terminal, run E2E tests
cd UnitTests
dotnet test --filter "PlaywrightE2ETests"
```

#### **Method 3: PowerShell with Options**
```powershell
# Only Playwright tests
./run-playwright-tests.ps1 -PlaywrightOnly

# Skip build if already built
./run-playwright-tests.ps1 -SkipBuild
```

---

### **Headless vs Headed Mode**

**Current Configuration: HEADLESS (Fast)**
```csharp
// In PlaywrightE2ETests.cs
Headless = true,      // No visible browser
Args = new[] { "--ignore-certificate-errors" }
```

**To Watch Tests Run (Headed Mode):**
Edit `PlaywrightE2ETests.cs`:
```csharp
Headless = false,     // Browser visible
SlowMo = 600,         // 600ms delay between actions
```

Then rebuild:
```bash
dotnet build
```

### **What the 9 E2E Tests Do**

1. **Test01** - Home page loads with Glork's greeting
2. **Test02** - Products page with warehouse background
3. **Test03** - Search functionality (filters products)
4. **Test04** - Product details page
5. **Test05** - Add to cart functionality
6. **Test06** - Cart page themed correctly
7. **Test07** - About page (Glork's story) + screenshot
8. **Test08** - All navigation links work
9. **Test09** - Complete user journey + screenshot

**Screenshots saved to:** `UnitTests/bin/Debug/net9.0/`
- `webshop-about-page.png`
- `complete-journey-final.png`

---

## ?? Generating Reports

### **Coverage Report (HTML)**

```bash
# Quick method
generate-coverage-report.bat

# Manual method
dotnet test --collect:"XPlat Code Coverage" --results-directory TestResults
reportgenerator -reports:TestResults/**/coverage.cobertura.xml -targetdir:CoverageReport -reporttypes:Html
start chrome CoverageReport/index.html
```

**Output:**
- Interactive HTML dashboard
- Line-by-line coverage highlighting
- Assembly/class/method breakdown
- Color-coded coverage bars
- Drill-down navigation

---

### **Coverage Report (Text Summary)**

```bash
reportgenerator \
  -reports:TestResults/**/coverage.cobertura.xml \
  -targetdir:CoverageReport \
  -reporttypes:TextSummary

cat CoverageReport/Summary.txt
```

**Output:**
```
Summary
  Line coverage: 95.8%
  Branch coverage: 83.3%
  Method coverage: 96.0%
  
  Services:      100.0%
  Data:          100.0%
  Models:         95.8%
  Controllers:    89.0%
```

---

### **Coverage Report (Badges)**

```bash
reportgenerator \
  -reports:TestResults/**/coverage.cobertura.xml \
  -targetdir:CoverageReport \
  -reporttypes:Badges

# Generates SVG badges for README
# CoverageReport/badge_linecoverage.svg
# CoverageReport/badge_branchcoverage.svg
```

---

### **Playwright Test Report**

Playwright tests output to console:
```bash
dotnet test --filter "PlaywrightE2ETests" --logger:"console;verbosity=detailed"
```

**Output:**
```
?? Testing Home Page...
? Home page loaded successfully!

???  Testing Products Page...
? Products page loaded!

?? Running Complete User Journey...
? Complete user journey test passed!
```

---

### **All Reports at Once**

```bash
# Run full test suite (generates all reports)
run-playwright.bat
```

**Generates:**
1. ? HTML coverage report ? `CoverageReport/index.html`
2. ? Test results ? Console output
3. ? Screenshots ? `UnitTests/bin/Debug/net9.0/*.png`
4. ? Coverage data ? `TestResults/**/*.xml`

---

## ?? Development

### **Project Architecture**

```
???????????????????????????????????????????
?         ASP.NET Core MVC App            ?
?  (Controllers, Views, wwwroot)          ?
???????????????????????????????????????????
               ?
               ?
???????????????????????????????????????????
?         Services Layer                  ?
?  (Business Logic, Cart, Products)       ?
???????????????????????????????????????????
               ?
               ?
???????????????????????????????????????????
?         Data Layer                      ?
?  (EF Core, DbContext, Repositories)     ?
???????????????????????????????????????????
               ?
               ?
???????????????????????????????????????????
?         SQLite Database                 ?
?  (Products, Categories, Orders)         ?
???????????????????????????????????????????
```

### **Clean Architecture Principles**
- ? **Separation of Concerns** - Each layer has one responsibility
- ? **Dependency Injection** - All services injected via DI
- ? **Testability** - Services can be mocked for unit tests
- ? **Repository Pattern** - Data access abstracted
- ? **Service Layer** - Business logic separated from controllers

### **Adding New Features**

1. **Create the Model** (if needed)
```csharp
// Models/Entities/NewEntity.cs
public class NewEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

2. **Update DbContext**
```csharp
// Data/WebshopDbContext.cs
public DbSet<NewEntity> NewEntities { get; set; }
```

3. **Create Migration**
```bash
dotnet ef migrations add AddNewEntity
dotnet ef database update
```

4. **Create Service Interface**
```csharp
// Services/Interfaces/INewService.cs
public interface INewService
{
    Task<List<NewEntity>> GetAllAsync();
}
```

5. **Implement Service**
```csharp
// Services/Implementations/NewService.cs
public class NewService : INewService
{
    // Implementation
}
```

6. **Register in DI**
```csharp
// Program.cs
builder.Services.AddScoped<INewService, NewService>();
```

7. **Create Controller**
```csharp
// Controllers/NewController.cs
public class NewController : Controller
{
    private readonly INewService _service;
    // ...
}
```

8. **Write Tests**
```csharp
// UnitTests/NewService_Tests.cs
public class NewService_Tests
{
    [Fact]
    public async Task GetAll_ReturnsAllItems()
    {
        // Arrange, Act, Assert
    }
}
```

### **Database Management**

**Create new migration:**
```bash
dotnet ef migrations add YourMigrationName
```

**Apply migrations:**
```bash
dotnet ef database update
```

**Reset database:**
```bash
dotnet ef database drop
dotnet ef database update
```

### **Environment Configuration**

Edit `appsettings.json` for:
- Database connection string
- Logging levels
- Application settings

For production, use `appsettings.Production.json`

---

## ?? Contributing

### **Development Workflow**

1. **Fork the repository**
2. **Create a feature branch**
```bash
git checkout -b feature/amazing-feature
```

3. **Make your changes**
4. **Write tests** for new features
5. **Run the test suite**
```bash
./run-playwright.bat
```

6. **Ensure coverage** doesn't drop
```bash
./generate-coverage-report.bat
```

7. **Commit your changes**
```bash
git commit -m "Add amazing feature"
```

8. **Push to your fork**
```bash
git push origin feature/amazing-feature
```

9. **Open a Pull Request**

### **Code Standards**

- ? Follow C# naming conventions
- ? Use `async/await` for async operations
- ? Keep methods small and focused
- ? Write XML documentation for public APIs
- ? Maintain test coverage above 80%
- ? Run code formatter before committing

### **Testing Requirements**

- ? All new code must have unit tests
- ? Integration tests for new endpoints
- ? Coverage must not drop below 80%
- ? All tests must pass before PR merge
- ? Add negative test scenarios for error paths

---

## ?? Documentation

- **[Coverage Guide](COVERAGE-GUIDE.md)** - Detailed coverage documentation

For detailed information about testing, coverage, and Playwright, see the sections above in this README.

---

## ?? Troubleshooting

### **Build Errors**
```bash
dotnet clean
dotnet restore
dotnet build
```

### **Database Issues**
```bash
dotnet ef database drop
dotnet ef database update
```

### **Test Failures**
- Make sure app is running for Playwright tests (`https://localhost:5001`)
- Check port 5001 isn't used by another app
- Clear `TestResults` and `CoverageReport` folders

### **Coverage Report Not Opening**
```bash
# Install reportgenerator
dotnet tool install -g dotnet-reportgenerator-globaltool

# Regenerate report
./generate-coverage-report.bat
```

### **Playwright Browser Issues**
```bash
# Install/update browsers
pwsh bin/Debug/net9.0/playwright.ps1 install

# Or use headed mode to see what's happening
# Edit PlaywrightE2ETests.cs: Headless = false
```

---

## ?? License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## ?? Acknowledgments

### **People**
- **The Big Glork** - Founder and Chief Space Parts Enthusiast
- **ASP.NET Core Team** - For the amazing framework
- **xUnit Team** - For the testing framework
- **.NET Community** - For inspiration and support

### **AI Tools**
- **GitHub Copilot** - For code suggestions and test generation
- **ChatGPT-4 (OpenAI)** - For architecture guidance and documentation
- **Claude Sonnet 3.5 (Anthropic)** - For comprehensive test analysis and coverage improvement

---

## ?? The Glork Guarantee

> *"In the year 2261, when I founded Nebula Space Parts from a single warehouse on Titan, I made a promise: Every component we sell meets the highest standards of the cosmos. From quantum hyperdrives to simple flux regulators, if it's from Glork... you can trust it to bring you home."*
>
> *"And now, with 95.8% code coverage and 732 passing tests, I can confidently say: This codebase meets those same standards. Whether you're debugging a shopping cart or deploying to production, this code will bring you home safely."*
>
> *"This project was built with the assistance of GitHub Copilot, ChatGPT-4, and Claude Sonnet 3.5—proving that even The Big Glork embraces modern tools. Test thoroughly. Code with precision. Use AI wisely. And remember: In space, no one can hear you debug... unless you have proper logging."*
>
> **— The Big Glork**  
> *Founder, Nebula Space Parts*  
> *Master of Quantum Commerce*  
> *Defender of Clean Code*  
> *Early Adopter of AI-Assisted Development*

---

<div align="center">

### ?? Built with precision. Tested with care. AI-assisted. Approved by Glork. ?

**[? Star this repo](https://github.com/Boundless-State/webshopAI)** | **[?? Report a bug](https://github.com/Boundless-State/webshopAI/issues)** | **[?? Request a feature](https://github.com/Boundless-State/webshopAI/issues)**

Made with ??, ?? AI assistance, and a sense of cosmic adventure

**AI Development Stats:**  
95.8% Coverage | 732 Tests | 40 Hours Saved | Built with Copilot, GPT-4 & Claude

</div>
