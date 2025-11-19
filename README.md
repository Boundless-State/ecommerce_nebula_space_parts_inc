# WebshopAI - Spacecraft Parts E-Commerce Platform

A comprehensive .NET 9 ASP.NET Core MVC e-commerce application for spacecraft parts with extensive test coverage and AI-assisted development.

## ?? Project Status

![Build Status](https://img.shields.io/badge/build-passing-brightgreen)
![Test Coverage](https://img.shields.io/badge/coverage-85%25-green)
![Tests](https://img.shields.io/badge/tests-600%2B-blue)
![.NET](https://img.shields.io/badge/.NET-9.0-purple)

## ?? Features

- **Product Catalog** - Browse spacecraft parts by category
- **Search & Filter** - Advanced product search capabilities
- **Shopping Cart** - Session-based cart management
- **Checkout** - Complete order placement workflow
- **Payment Integration** - Mock payment provider (extensible to Stripe)
- **Responsive Design** - Works on mobile, tablet, and desktop

## ?? Testing Strategy

This project demonstrates **professional-grade testing** with multiple automated test methods:

### Test Coverage

| Metric | Target | Current | Status |
|--------|--------|---------|--------|
| **Line Coverage** | 85% | 85%+ | ? Met |
| **Branch Coverage** | 80% | 80%+ | ? Met |
| **Total Tests** | 500+ | 600+ | ? Exceeded |

### Test Methods

1. **Unit Tests** (~570 tests)
   - Service layer business logic
   - Controller actions
   - ViewModels and DTOs
   - Branch coverage for all conditionals

2. **Integration Tests** (~20 tests)
   - HTTP endpoint testing
   - Full application stack validation
   - Database integration

3. **API Tests** (~25 tests)
   - HTTP status codes
   - Response validation
   - Endpoint contracts

4. **E2E Tests** (~15 tests)
   - Complete user journeys
   - UI interaction with Playwright
   - Responsive design validation

## ?? Project Structure

```
webshopAI/
??? webshopAI/           # Main web application (Controllers, Views)
??? Data/                # EF Core DbContext and migrations
??? Models/              # Entities and ViewModels
??? Services/            # Business logic layer
??? UnitTests/           # Unit tests (570+ tests)
??? IntegrationTests/    # Integration and E2E tests (40+ tests)
??? docs/                # Documentation
    ??? TestStrategy.md           # Comprehensive test strategy
    ??? AI-Usage-Reflection.md    # AI usage analysis
    ??? Coverage-Justification.md # Coverage philosophy
```

## ??? Technologies

- **.NET 9** - Latest framework
- **ASP.NET Core MVC** - Web framework
- **Entity Framework Core** - ORM
- **SQL Server** - Database
- **xUnit** - Test framework
- **Moq** - Mocking framework
- **Playwright** - E2E testing
- **Coverlet** - Code coverage
- **ReportGenerator** - Coverage reports

## ?? AI-Assisted Development

This project extensively used AI tools to accelerate development:

- **GitHub Copilot** - Code generation, test creation (60% of tests)
- **ChatGPT/Claude** - Architecture decisions, research
- **Impact** - 40-50% faster development, 85% line coverage achieved

**Key Learnings:**
- ? AI excels at patterns and boilerplate
- ? Massive time savings in test generation
- ?? Human oversight essential for business logic
- ?? AI-generated code requires validation

See [AI-Usage-Reflection.md](./docs/AI-Usage-Reflection.md) for detailed analysis.

## ?? Getting Started

### Prerequisites

- .NET 9 SDK
- SQL Server (LocalDB or full)
- (Optional) Playwright browsers for E2E tests

### Installation

```bash
# Clone repository
git clone https://github.com/Boundless-State/webshopAI.git
cd webshopAI

# Restore packages
dotnet restore

# Update database
cd webshopAI
dotnet ef database update

# Run application
dotnet run
```

Application will be available at `https://localhost:5001`

### Running Tests

```bash
# Run all tests
dotnet test

# Run unit tests only
dotnet test UnitTests/UnitTests.csproj

# Run integration tests
dotnet test IntegrationTests/IntegrationTests.csproj

# Run with coverage
PowerShell -ExecutionPolicy Bypass -File generate-coverage.ps1
```

### Generate Coverage Report

```bash
cd webshopAI

# Run script (installs tools if needed, runs tests, opens report)
PowerShell -ExecutionPolicy Bypass -File generate-coverage.ps1
```

Report opens automatically in Chrome at `TestResults/CoverageReport/index.html`

## ?? Documentation

- **[Test Strategy](./docs/TestStrategy.md)** - Comprehensive testing approach
- **[AI Usage Reflection](./docs/AI-Usage-Reflection.md)** - AI impact analysis
- **[Coverage Justification](./docs/Coverage-Justification.md)** - What we test and why

## ?? Test Examples

### Unit Test (AAA Pattern)
```csharp
[Fact]
public async Task GetByIdAsync_ReturnsProduct_WhenExists()
{
    // Arrange
    using var db = CreateDb();
    var product = new Product { Id = 1, Name = "Test", IsActive = true };
    db.Products.Add(product);
    await db.SaveChangesAsync();
    IProductService svc = new ProductService(db, logger);
    
    // Act
    var result = await svc.GetByIdAsync(1);
    
    // Assert
    Assert.NotNull(result);
}
```

### Integration Test
```csharp
[Fact]
public async Task Products_Index_ReturnsOK()
{
    var client = _factory.CreateClient();
    var response = await client.GetAsync("/Products");
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
}
```

### E2E Test (Playwright)
```csharp
[Fact]
public async Task CompleteCheckout_CreatesOrder()
{
    var page = await _browser.NewPageAsync();
    await page.GotoAsync("/Products");
    await page.ClickAsync("button:has-text('Add to Cart')");
    await page.GotoAsync("/Checkout");
    await page.FillAsync("#customerName", "John Doe");
    await page.ClickAsync("button:has-text('Place Order')");
    await page.WaitForSelectorAsync("text=Order Confirmed");
}
```

## ?? Coverage Philosophy

> "Code coverage is a measure of testing completeness, not quality. We aim for **meaningful coverage** of testable, valuable code."

**What We Test:**
- ? Business logic in services
- ? Controller actions
- ? Validation logic
- ? Error handling paths

**What We Exclude:**
- ? EF Core infrastructure
- ? Auto-generated migrations
- ? POCO entities (no logic)
- ? Simple property getters/setters

See [Coverage-Justification.md](./docs/Coverage-Justification.md) for details.

## ?? Test Metrics

| Test Type | Count | Execution Time |
|-----------|-------|----------------|
| Unit Tests | 570+ | ~8 seconds |
| Integration Tests | 20+ | ~5 seconds |
| API Tests | 25+ | ~3 seconds |
| E2E Tests (Playwright) | 15+ | ~10 seconds |
| **Total** | **630+** | **~26 seconds** |

## ?? Best Practices Demonstrated

1. **AAA Pattern** - Arrange-Act-Assert in all tests
2. **Single Assertion** - One logical assertion per test
3. **Descriptive Names** - `MethodName_Scenario_ExpectedBehavior`
4. **Mocking** - Isolate units under test
5. **Branch Coverage** - Test all conditional paths
6. **Negative Tests** - Error scenarios and edge cases
7. **E2E Tests** - Complete user journeys
8. **CI/CD Ready** - Fast, reliable, automated

## ?? Contributing

1. Fork the repository
2. Create a feature branch
3. Write tests first (TDD)
4. Ensure 85%+ coverage
5. Submit pull request

## ?? License

This project is for educational purposes.

## ?? Team

- **Development** - AI-assisted development with GitHub Copilot
- **Testing** - Comprehensive automated test suite
- **Documentation** - Complete strategy and reflection docs

## ?? Links

- **Repository**: https://github.com/Boundless-State/webshopAI
- **Test Strategy**: [docs/TestStrategy.md](./docs/TestStrategy.md)
- **AI Reflection**: [docs/AI-Usage-Reflection.md](./docs/AI-Usage-Reflection.md)

---

**Built with .NET 9, tested with passion, documented with care.** ??
