# Test Strategy Document

## WebshopAI E-Commerce Application Test Strategy

**Version:** 1.0  
**Date:** 2025-01-05  
**Author:** Development Team  
**Project:** WebshopAI - Spacecraft Parts E-Commerce Platform

---

## 1. Executive Summary

This document outlines the comprehensive testing strategy for the WebshopAI application, a .NET 9 ASP.NET Core MVC-based e-commerce platform for spacecraft parts. Our testing approach ensures high quality, reliability, and maintainability through multiple automated testing layers.

**Key Metrics:**
- **Total Tests:** 560+
- **Line Coverage:** 85%+
- **Branch Coverage:** Target 80-90%
- **Test Methods:** 4 (Unit, Integration, API, E2E)

---

## 2. Testing Pyramid

```
           /\
          /E2E\        ~10 tests (UI automation)
         /------\
        /  API   \     ~20 tests (endpoint validation)
       /----------\
      /Integration \   ~30 tests (component interaction)
     /--------------\
    /   Unit Tests   \ ~500 tests (business logic)
   /------------------\
```

### Test Distribution Philosophy
- **70%** Unit Tests - Fast, isolated, business logic
- **20%** Integration Tests - Component interaction
- **5%** API Tests - Endpoint contracts
- **5%** E2E Tests - Critical user journeys

---

## 3. Test Methods & Technologies

### 3.1 Unit Testing
**Framework:** xUnit  
**Mocking:** Moq  
**Coverage:** Coverlet

**Scope:**
- Business logic in Services layer
- Controller action methods
- ViewModels and DTOs
- Entity model validation
- Helper methods and utilities

**Pattern:** AAA (Arrange-Act-Assert)
- Single responsibility per test
- One assertion per test
- Descriptive test names

**Example:**
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

### 3.2 Integration Testing
**Framework:** xUnit + Microsoft.AspNetCore.Mvc.Testing  
**Infrastructure:** WebApplicationFactory

**Scope:**
- HTTP endpoints (GET, POST)
- Middleware pipeline
- Dependency injection
- Database integration
- Session management
- Authentication/Authorization

**Example:**
```csharp
[Fact]
public async Task Products_Index_ReturnsOK()
{
    // Arrange
    var client = _factory.CreateClient();
    
    // Act
    var response = await client.GetAsync("/Products");
    
    // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
}
```

### 3.3 API Testing
**Framework:** xUnit + WebApplicationFactory  
**Tools:** HTTP Client, JSON validation

**Scope:**
- RESTful endpoint contracts
- HTTP status codes
- Request/response validation
- Content-Type headers
- Error responses
- Data serialization

**Endpoints Tested:**
- `GET /Products` - List products
- `GET /Products/{id}` - Get product details
- `POST /Cart/Add` - Add to cart
- `POST /Checkout/PlaceOrder` - Place order
- Error scenarios (404, 400, 500)

### 3.4 End-to-End Testing
**Framework:** Playwright + xUnit  
**Browsers:** Chromium, Firefox, WebKit

**Scope:**
- Complete user journeys
- Multi-page workflows
- Form submissions
- JavaScript interactions
- Visual regression (future)
- Accessibility (future)

**Critical Flows:**
1. Browse ? Add to Cart ? Checkout ? Order Confirmation
2. Search Products ? Filter ? View Details
3. Cart Management (Add, Update, Remove)
4. Error handling and validation

**Example:**
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

---

## 4. Test Coverage Strategy

### 4.1 Coverage Targets

| Metric | Target | Current | Status |
|--------|--------|---------|--------|
| Line Coverage | 85% | 85% | ? Met |
| Branch Coverage | 80% | 25% | ?? In Progress |
| Method Coverage | 75% | 76% | ? Met |

### 4.2 Coverage Configuration

**Included Assemblies:**
- `[Services]*` - Business logic
- `[webshopAPP]*` - Controllers and infrastructure

**Excluded Code:**
- `[Data]*` - EF Core DbContext and migrations
- `[Models]*` - POCO entities and ViewModels
- `AspNetCore.*` - Razor generated code
- `Program` - Host bootstrapping
- `[*.Tests]*` - Test code itself

**Justification:** See [Coverage-Justification.md](./Coverage-Justification.md)

### 4.3 Coverage Tools
- **Coverlet.msbuild** - Coverage collection
- **ReportGenerator** - HTML reports
- **Directory.Build.props** - Global coverage settings

---

## 5. Test Data Management

### 5.1 Unit Tests
- **Strategy:** In-memory database (EF Core)
- **Isolation:** New database per test (Guid-based names)
- **Seed Data:** Minimal, per-test setup
- **Cleanup:** Automatic disposal via `using`

### 5.2 Integration Tests
- **Strategy:** SQL Server LocalDB with migrations
- **Isolation:** Test database per run
- **Seed Data:** Migrations + data seeding
- **Cleanup:** Database dropped after test run

### 5.3 E2E Tests
- **Strategy:** Full application with real database
- **Isolation:** Transaction rollback or database reset
- **Seed Data:** Realistic production-like data
- **Cleanup:** Automated cleanup scripts

---

## 6. Mocking Strategy

### 6.1 When to Mock
? **Mock When:**
- External dependencies (payment gateways, APIs)
- Database for unit tests
- HTTP context and session
- File system operations
- Time-dependent logic

? **Don't Mock:**
- Simple DTOs or POCOs
- Internal business logic
- Value objects
- Pure functions

### 6.2 Mocking Tools
- **Moq** - Primary mocking framework
- **In-Memory Database** - EF Core mocking
- **TestSession** - Custom session implementation

**Example:**
```csharp
var mockPayment = new Mock<IPaymentService>();
mockPayment
    .Setup(p => p.ChargeAsync(It.IsAny<decimal>(), It.IsAny<PaymentRequest>(), default))
    .ReturnsAsync(new PaymentResult(true, "TXN123"));
```

---

## 7. Positive & Negative Test Scenarios

### 7.1 Positive Scenarios (Happy Path)
- ? Valid user input
- ? Successful operations
- ? Expected data flows
- ? Normal user journeys

**Coverage:** ~70% of tests

### 7.2 Negative Scenarios (Error Paths)
- ? Invalid input (null, empty, malformed)
- ? Business rule violations
- ? External service failures
- ? Unauthorized access
- ? Network errors
- ? Database failures
- ? Concurrent access issues

**Coverage:** ~30% of tests

**Example Negative Tests:**
```csharp
[Fact]
public async Task PlaceOrder_WithFailedPayment_RedirectsToCheckout()
{
    // Arrange
    var paymentMock = new Mock<IPaymentService>();
    paymentMock
        .Setup(p => p.ChargeAsync(It.IsAny<decimal>(), It.IsAny<PaymentRequest>(), default))
        .ReturnsAsync(new PaymentResult(false, null, "Card declined"));
    
    // Act
    var result = await controller.PlaceOrder("John", "john@test.com", "123 St");
    
    // Assert
    var redirect = Assert.IsType<RedirectToActionResult>(result);
    Assert.Equal("Index", redirect.ActionName);
}
```

---

## 8. Continuous Integration

### 8.1 CI/CD Pipeline (GitHub Actions)

```yaml
- name: Run Tests
  run: dotnet test --configuration Release
  
- name: Generate Coverage
  run: |
    dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
    reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage-report
    
- name: Upload Coverage
  uses: codecov/codecov-action@v3
  with:
    files: ./coverage.cobertura.xml
```

### 8.2 Quality Gates
- ? All tests must pass
- ? Line coverage ? 85%
- ? Branch coverage ? 80%
- ? No critical code smells
- ? Build succeeds

---

## 9. Test Execution

### 9.1 Local Development

**Run all tests:**
```powershell
dotnet test
```

**Run with coverage:**
```powershell
PowerShell -ExecutionPolicy Bypass -File .\generate-coverage.ps1
```

**Run specific test types:**
```powershell
# Unit tests only
dotnet test UnitTests/UnitTests.csproj

# Integration tests only
dotnet test IntegrationTests/IntegrationTests.csproj

# Playwright tests only
dotnet test --filter "FullyQualifiedName~Playwright"
```

### 9.2 CI/CD Execution
- **Trigger:** Every push and PR
- **Parallel:** Yes (per test project)
- **Timeout:** 10 minutes
- **Retry:** Failed tests retry once

---

## 10. Test Maintenance

### 10.1 Naming Conventions
- **Pattern:** `MethodName_Scenario_ExpectedBehavior`
- **Example:** `GetByIdAsync_ReturnsNull_WhenProductNotFound`

### 10.2 Test Organization
```
UnitTests/
??? Services/
?   ??? ProductServiceTests.cs
?   ??? CartServiceTests.cs
?   ??? PaymentServiceTests.cs
??? Controllers/
?   ??? HomeControllerTests.cs
?   ??? ProductsControllerTests.cs
?   ??? CheckoutControllerTests.cs
??? Models/
    ??? EntityModelsTests.cs
    ??? ViewModelsTests.cs

IntegrationTests/
??? API/
?   ??? ProductsApiTests.cs
?   ??? CartApiTests.cs
??? E2E/
?   ??? PlaywrightE2ETests.cs
??? SmokeTests.cs
```

### 10.3 Code Review Checklist
- [ ] Test follows AAA pattern
- [ ] Single assertion per test
- [ ] Descriptive test name
- [ ] No hard-coded values
- [ ] Proper cleanup (using/IAsyncLifetime)
- [ ] Fast execution (< 1s for unit tests)

---

## 11. Future Improvements

### 11.1 Short Term (Q1 2025)
- [ ] Increase branch coverage to 90%
- [ ] Add more negative scenario tests
- [ ] Implement visual regression testing
- [ ] Add performance benchmarks

### 11.2 Long Term (Q2-Q4 2025)
- [ ] Load testing (k6, JMeter)
- [ ] Security testing (OWASP ZAP)
- [ ] Accessibility testing (axe-core)
- [ ] Mutation testing (Stryker.NET)
- [ ] Contract testing (Pact)

---

## 12. Metrics & Reporting

### 12.1 Test Metrics Dashboard
- Total test count
- Pass/fail rate
- Execution time trends
- Coverage trends
- Flaky test tracking

### 12.2 Coverage Reports
- **Location:** `TestResults/CoverageReport/index.html`
- **Format:** HTML + Cobertura XML
- **Frequency:** Every test run
- **Artifacts:** Uploaded to CI/CD

---

## 13. Roles & Responsibilities

| Role | Responsibility |
|------|----------------|
| **Developers** | Write unit tests for new code, maintain existing tests |
| **QA Engineers** | Write E2E tests, review test coverage |
| **DevOps** | Maintain CI/CD pipeline, monitor test execution |
| **Tech Lead** | Review test strategy, enforce quality gates |

---

## 14. Appendices

### A. Test Tools Reference
- **xUnit** - https://xunit.net/
- **Moq** - https://github.com/moq/moq4
- **Playwright** - https://playwright.dev/dotnet/
- **Coverlet** - https://github.com/coverlet-coverage/coverlet
- **ReportGenerator** - https://github.com/danielpalme/ReportGenerator

### B. Related Documents
- [Coverage-Justification.md](./Coverage-Justification.md)
- [AI-Usage-Reflection.md](./AI-Usage-Reflection.md)
- [README.md](../README.md)

### C. Contact
For questions about testing strategy:
- GitHub Issues: https://github.com/Boundless-State/webshopAI/issues
- Team Lead: [Your Name]

---

**Document Status:** Living Document - Updated continuously  
**Last Review:** 2025-01-05  
**Next Review:** 2025-02-05
