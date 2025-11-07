# ?? Quick Start - Running Playwright E2E Tests

## ? Prerequisites

1. **App must be running** at `https://localhost:5001`
2. **Playwright browsers installed** (auto-installs on first run)

---

## ?? Quick Commands

### **Run All Tests (Recommended)**
```bash
# Windows
run-playwright.bat

# PowerShell/Linux/Mac
./run-playwright-tests.ps1
```

This runs:
- ? Unit tests (676)
- ? Integration tests (47)
- ? Playwright E2E tests (9)
- ? Generates coverage report

---

### **Run Only E2E Tests**

**Step 1: Start the app**
```bash
cd webshopAI
dotnet run
```

**Step 2: Run E2E tests (in another terminal)**
```bash
cd UnitTests
dotnet test --filter "PlaywrightE2ETests"
```

---

## ?? Expected Output

```
?? Testing Home Page with Spaceport Station Background...
? Home page loaded successfully with spaceport background!

???  Testing Products Page with Warehouse Background...
? Products page loaded with warehouse background!

?? Testing Search Functionality...
? Search functionality working!

?? Testing Product Details Page...
? Product details page loaded!

?? Testing Add to Cart Functionality...
? Product added to cart!

?? Testing Cart Page Theme...
? Cart page loaded with correct theme!

??  Testing About Page with Glork's Story...
? About page shows complete Glork story with spaceport background!

?? Testing Navigation Links...
? All navigation links working!

?? Running Complete User Journey Test (Headless)...
   This simulates a real customer browsing the shop
   Step 1: Landing on home page...
   Step 2: Browsing products...
   Step 3: Searching for Quantum parts...
   Step 4: Viewing product details...
   Step 5: Adding to cart...
   Step 6: Reviewing cart...
   Step 7: Learning about Nebula Space Parts...
? Complete user journey test passed!

Test Run Successful.
Total tests: 9
     Passed: 9
```

---

## ?? Troubleshooting

### **Error: "ERR_CONNECTION_REFUSED"**
**Problem:** App isn't running

**Solution:**
```bash
cd webshopAI
dotnet run
```
Wait for "Now listening on: https://localhost:5001"

---

### **Error: "Playwright not available"**
**Problem:** Browsers not installed

**Solution:**
```bash
pwsh bin/Debug/net9.0/playwright.ps1 install
```

Or let tests auto-install (first run is slower)

---

### **Error: "Timeout waiting for page"**
**Problem:** App is slow to start or not responding

**Solution:**
1. Check app is running: `https://localhost:5001`
2. Increase wait time in script:
   - Edit `run-playwright-tests.ps1`
   - Change `WaitSeconds = 10` ? `WaitSeconds = 15`

---

### **Want to See the Browser? (Debugging)**

Edit `UnitTests/PlaywrightE2ETests.cs`:

```csharp
_browser = await _playwright.Chromium.LaunchAsync(new()
{
    Headless = false,     // Browser window visible
    SlowMo = 600,         // Slow down actions
    Args = new[] { "--ignore-certificate-errors" }
});
```

Then rebuild:
```bash
dotnet build
```

---

## ?? Screenshots

Tests generate screenshots in:
```
UnitTests/bin/Debug/net9.0/webshop-about-page.png
UnitTests/bin/Debug/net9.0/complete-journey-final.png
```

Even in headless mode! ??

---

## ?? Quick Reference

| Command | What It Does |
|---------|--------------|
| `run-playwright.bat` | Full test suite (Windows) |
| `./run-playwright-tests.ps1` | Full test suite (PowerShell) |
| `dotnet test --filter "PlaywrightE2ETests"` | E2E tests only |
| `pwsh bin/Debug/net9.0/playwright.ps1 install` | Install browsers |
| `dotnet run` (in webshopAI folder) | Start the app |

---

**Happy testing! ??**

If tests fail, check:
1. ? App is running at `https://localhost:5001`
2. ? Playwright browsers are installed
3. ? Port 5001 isn't used by another process
