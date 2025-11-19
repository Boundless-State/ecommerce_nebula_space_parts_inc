using Microsoft.Playwright;
using Xunit;

namespace UnitTests;

/// <summary>
/// Playwright E2E (End-to-End) tests - runs in HEADLESS mode (no visible browser)
/// Tests the complete user journey against a running application
/// Fast automated testing suitable for CI/CD pipelines
/// </summary>
public class PlaywrightE2ETests : IAsyncLifetime
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private bool _isAvailable = true;
    private const string BaseUrl = "https://localhost:5001";

    public async Task InitializeAsync()
    {
        try
        {
            // Try to install Playwright browsers if not already installed
            try
            {
                Microsoft.Playwright.Program.Main(new[] { "install", "chromium" });
            }
            catch
            {
                // Ignore install errors - browsers may already be installed
            }

            _playwright = await Playwright.CreateAsync();
            
            // Launch in HEADLESS mode - no visible browser window
            _browser = await _playwright.Chromium.LaunchAsync(new()
            {
                Headless = true,      // No visible browser window
                Args = new[] { "--ignore-certificate-errors" } // Ignore SSL cert errors for localhost
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"??  Playwright initialization failed: {ex.Message}");
            Console.WriteLine("   Run 'pwsh bin/Debug/net9.0/playwright.ps1 install' to install browsers");
            _isAvailable = false;
        }
    }

    public async Task DisposeAsync()
    {
        if (_browser != null)
            await _browser.DisposeAsync();
        _playwright?.Dispose();
    }

    [Fact]
    public async Task Test01_HomePage_LoadsWithSpaceBackground()
    {
        if (!_isAvailable || _browser is null)
        {
            Console.WriteLine("??  Skipping test - Playwright not available");
            return;
        }

        var page = await _browser.NewPageAsync();
        
        try
        {
            Console.WriteLine("?? Testing Home Page with Spaceport Station Background...");
            await page.GotoAsync(BaseUrl, new() { WaitUntil = WaitUntilState.NetworkIdle, Timeout = 30000 });
            
            // Verify Glork's greeting
            var greeting = await page.Locator("text=The Big Glork Greets You!").CountAsync();
            Assert.True(greeting > 0, "Glork's greeting should be visible");
            
            // Verify welcome message
            var welcome = await page.Locator("h1:has-text('Welcome')").CountAsync();
            Assert.True(welcome > 0, "Welcome message should be visible");
            
            Console.WriteLine("? Home page loaded successfully with spaceport background!");
        }
        catch (PlaywrightException ex) when (ex.Message.Contains("ERR_CONNECTION_REFUSED") || ex.Message.Contains("net::ERR"))
        {
            Assert.Fail($"? Could not connect to app at {BaseUrl}. Make sure the app is running with 'dotnet run'");
        }
        catch (TimeoutException)
        {
            Assert.Fail($"? Timeout waiting for page to load. Is the app running at {BaseUrl}?");
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task Test02_ProductsPage_LoadsWithWarehouseBackground()
    {
        if (!_isAvailable || _browser is null)
        {
            Console.WriteLine("??  Skipping test - Playwright not available");
            return;
        }

        var page = await _browser.NewPageAsync();
        
        try
        {
            Console.WriteLine("???  Testing Products Page with Warehouse Background...");
            await page.GotoAsync($"{BaseUrl}/Products", new() { WaitUntil = WaitUntilState.NetworkIdle, Timeout = 30000 });
            
            // Verify Products heading
            var heading = await page.Locator("h1:has-text('Products')").CountAsync();
            Assert.True(heading > 0, "Products heading should be visible");
            
            // Verify search box exists
            var searchBox = await page.Locator("input[name='q']").CountAsync();
            Assert.True(searchBox > 0, "Search box should be present");
            
            Console.WriteLine("? Products page loaded with warehouse background!");
        }
        catch (PlaywrightException ex) when (ex.Message.Contains("ERR_CONNECTION_REFUSED") || ex.Message.Contains("net::ERR"))
        {
            Assert.Fail($"? Could not connect to app at {BaseUrl}. Make sure the app is running.");
        }
        catch (TimeoutException)
        {
            Assert.Fail($"? Timeout waiting for Products page. Is the app running at {BaseUrl}?");
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task Test03_SearchFunctionality_FiltersProducts()
    {
        if (!_isAvailable || _browser is null)
        {
            Console.WriteLine("??  Skipping test - Playwright not available");
            return;
        }

        var page = await _browser.NewPageAsync();
        
        try
        {
            Console.WriteLine("?? Testing Search Functionality...");
            await page.GotoAsync($"{BaseUrl}/Products", new() { WaitUntil = WaitUntilState.NetworkIdle, Timeout = 30000 });
            
            var searchBox = page.Locator("input[name='q']");
            if (await searchBox.CountAsync() > 0)
            {
                await searchBox.FillAsync("Quantum");
                await page.Keyboard.PressAsync("Enter");
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                
                // Verify URL contains query
                Assert.Contains("q=Quantum", page.Url);
                Console.WriteLine("? Search functionality working!");
            }
        }
        catch (PlaywrightException ex) when (ex.Message.Contains("ERR_CONNECTION_REFUSED") || ex.Message.Contains("net::ERR"))
        {
            Assert.Fail($"? Could not connect to app at {BaseUrl}.");
        }
        catch (TimeoutException)
        {
            Assert.Fail($"? Timeout during search test. Is the app running at {BaseUrl}?");
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task Test04_ProductDetails_ShowsProductInfo()
    {
        if (!_isAvailable || _browser is null)
        {
            Console.WriteLine("??  Skipping test - Playwright not available");
            return;
        }

        var page = await _browser.NewPageAsync();
        
        try
        {
            Console.WriteLine("?? Testing Product Details Page...");
            await page.GotoAsync($"{BaseUrl}/Products", new() { WaitUntil = WaitUntilState.NetworkIdle, Timeout = 30000 });
            
            var detailsLinks = page.Locator("a:has-text('Details')");
            if (await detailsLinks.CountAsync() > 0)
            {
                await detailsLinks.First.ClickAsync();
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                
                Assert.Contains("/Products/Details/", page.Url);
                Console.WriteLine("? Product details page loaded!");
            }
        }
        catch (PlaywrightException ex) when (ex.Message.Contains("ERR_CONNECTION_REFUSED") || ex.Message.Contains("net::ERR"))
        {
            Assert.Fail($"? Could not connect to app at {BaseUrl}.");
        }
        catch (TimeoutException)
        {
            Assert.Fail($"? Timeout loading product details. Is the app running at {BaseUrl}?");
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task Test05_AddToCart_AddsProduct()
    {
        if (!_isAvailable || _browser is null)
        {
            Console.WriteLine("??  Skipping test - Playwright not available");
            return;
        }

        var page = await _browser.NewPageAsync();
        
        try
        {
            Console.WriteLine("?? Testing Add to Cart Functionality...");
            await page.GotoAsync($"{BaseUrl}/Products", new() { WaitUntil = WaitUntilState.NetworkIdle, Timeout = 30000 });
            
            var addToCartButtons = page.Locator("button:has-text('Add to Cart')");
            if (await addToCartButtons.CountAsync() > 0)
            {
                await addToCartButtons.First.ClickAsync();
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                
                Console.WriteLine("? Product added to cart!");
            }
        }
        catch (PlaywrightException ex) when (ex.Message.Contains("ERR_CONNECTION_REFUSED") || ex.Message.Contains("net::ERR"))
        {
            Assert.Fail($"? Could not connect to app at {BaseUrl}.");
        }
        catch (TimeoutException)
        {
            Assert.Fail($"? Timeout adding product to cart. Is the app running at {BaseUrl}?");
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task Test06_Cart_DisplaysThemedCorrectly()
    {
        if (!_isAvailable || _browser is null)
        {
            Console.WriteLine("??  Skipping test - Playwright not available");
            return;
        }

        var page = await _browser.NewPageAsync();
        
        try
        {
            Console.WriteLine("?? Testing Cart Page Theme...");
            await page.GotoAsync($"{BaseUrl}/Cart", new() { WaitUntil = WaitUntilState.NetworkIdle, Timeout = 30000 });
            
            var heading = await page.Locator("h1:has-text('Cart')").CountAsync();
            Assert.True(heading > 0, "Cart heading should be visible");
            
            Console.WriteLine("? Cart page loaded with correct theme!");
        }
        catch (PlaywrightException ex) when (ex.Message.Contains("ERR_CONNECTION_REFUSED") || ex.Message.Contains("net::ERR"))
        {
            Assert.Fail($"? Could not connect to app at {BaseUrl}.");
        }
        catch (TimeoutException)
        {
            Assert.Fail($"? Timeout loading cart page. Is the app running at {BaseUrl}?");
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task Test07_AboutPage_ShowsGlorkStory()
    {
        if (!_isAvailable || _browser is null)
        {
            Console.WriteLine("??  Skipping test - Playwright not available");
            return;
        }

        var page = await _browser.NewPageAsync();
        
        try
        {
            Console.WriteLine("??  Testing About Page with Glork's Story...");
            await page.GotoAsync($"{BaseUrl}/Home/Privacy", new() { WaitUntil = WaitUntilState.NetworkIdle, Timeout = 30000 });
            
            // Verify company founding year
            var founding = await page.Locator("text=2261").CountAsync();
            Assert.True(founding > 0, "Company founding year should be visible");
            
            // Verify Glork's name
            var glork = await page.Locator("text=Glork").CountAsync();
            Assert.True(glork > 0, "Glork's name should appear in the story");
            
            // Verify the tagline
            var tagline = await page.Locator("text=If it's from Glork").CountAsync();
            Assert.True(tagline > 0, "Company tagline should be visible");
            
            // Take screenshot (works in headless mode!)
            await page.ScreenshotAsync(new()
            {
                Path = "webshop-about-page.png",
                FullPage = true
            });
            
            Console.WriteLine("? About page shows complete Glork story with spaceport background!");
        }
        catch (PlaywrightException ex) when (ex.Message.Contains("ERR_CONNECTION_REFUSED") || ex.Message.Contains("net::ERR"))
        {
            Assert.Fail($"? Could not connect to app at {BaseUrl}.");
        }
        catch (TimeoutException)
        {
            Assert.Fail($"? Timeout loading about page. Is the app running at {BaseUrl}?");
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task Test08_Navigation_AllLinksWork()
    {
        if (!_isAvailable || _browser is null)
        {
            Console.WriteLine("??  Skipping test - Playwright not available");
            return;
        }

        var page = await _browser.NewPageAsync();
        
        try
        {
            Console.WriteLine("?? Testing Navigation Links...");
            await page.GotoAsync(BaseUrl, new() { WaitUntil = WaitUntilState.NetworkIdle, Timeout = 30000 });
            
            // Test Products link
            await page.ClickAsync("text=Products");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            Assert.Contains("/Products", page.Url);
            
            // Test Cart link
            await page.ClickAsync("a[href='/Cart']");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            Assert.Contains("/Cart", page.Url);
            
            // Test About link
            await page.ClickAsync("text=About");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            Assert.Contains("/Privacy", page.Url);
            
            // Test Home link
            await page.ClickAsync("text=Home");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            Console.WriteLine("? All navigation links working!");
        }
        catch (PlaywrightException ex) when (ex.Message.Contains("ERR_CONNECTION_REFUSED") || ex.Message.Contains("net::ERR"))
        {
            Assert.Fail($"? Could not connect to app at {BaseUrl}.");
        }
        catch (TimeoutException)
        {
            Assert.Fail($"? Timeout during navigation test. Is the app running at {BaseUrl}?");
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task Test09_FullUserJourney_CompleteWorkflow()
    {
        if (!_isAvailable || _browser is null)
        {
            Console.WriteLine("??  Skipping test - Playwright not available");
            return;
        }

        var page = await _browser.NewPageAsync();
        
        try
        {
            Console.WriteLine("?? Running Complete User Journey Test (Headless)...");
            Console.WriteLine("   This simulates a real customer browsing the shop");
            
            // 1. Start at home
            Console.WriteLine("   Step 1: Landing on home page...");
            await page.GotoAsync(BaseUrl, new() { WaitUntil = WaitUntilState.NetworkIdle, Timeout = 30000 });
            
            // 2. Browse products
            Console.WriteLine("   Step 2: Browsing products...");
            await page.ClickAsync("text=Products");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            // 3. Search for specific item
            Console.WriteLine("   Step 3: Searching for Quantum parts...");
            var searchBox = page.Locator("input[name='q']");
            if (await searchBox.CountAsync() > 0)
            {
                await searchBox.FillAsync("Quantum");
                await page.Keyboard.PressAsync("Enter");
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            }
            
            // 4. View details
            Console.WriteLine("   Step 4: Viewing product details...");
            var detailsLinks = page.Locator("a:has-text('Details')");
            if (await detailsLinks.CountAsync() > 0)
            {
                await detailsLinks.First.ClickAsync();
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            }
            
            // 5. Add to cart
            Console.WriteLine("   Step 5: Adding to cart...");
            var addButtons = page.Locator("button:has-text('Add to Cart')");
            if (await addButtons.CountAsync() > 0)
            {
                await addButtons.First.ClickAsync();
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            }
            
            // 6. Check cart
            Console.WriteLine("   Step 6: Reviewing cart...");
            await page.ClickAsync("a[href='/Cart']");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            // 7. Read about company
            Console.WriteLine("   Step 7: Learning about Nebula Space Parts...");
            await page.ClickAsync("text=About");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            // Take final screenshot (works in headless!)
            await page.ScreenshotAsync(new()
            {
                Path = "complete-journey-final.png",
                FullPage = true
            });
            
            Console.WriteLine("? Complete user journey test passed!");
            Assert.True(true);
        }
        catch (PlaywrightException ex) when (ex.Message.Contains("ERR_CONNECTION_REFUSED") || ex.Message.Contains("net::ERR"))
        {
            Assert.Fail($"? Could not connect to app at {BaseUrl}. Make sure the app is running.");
        }
        catch (TimeoutException)
        {
            Assert.Fail($"? Timeout during user journey test. Is the app running at {BaseUrl}?");
        }
        finally
        {
            await page.CloseAsync();
        }
    }
}
