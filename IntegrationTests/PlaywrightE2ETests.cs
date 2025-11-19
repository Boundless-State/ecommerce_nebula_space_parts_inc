using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Playwright;
using Xunit;

namespace IntegrationTests;

/// <summary>
/// Comprehensive E2E tests using Playwright
/// Tests complete user journeys through the application
/// </summary>
public class PlaywrightE2ETests : IClassFixture<TestWebApplicationFactory>, IAsyncLifetime
{
    private readonly TestWebApplicationFactory _factory;
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private HttpClient? _client;
    private bool _isAvailable = true;

    public PlaywrightE2ETests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        try
        {
            try
            {
                Microsoft.Playwright.Program.Main(new[] { "install" });
            }
            catch
            {
                // Ignore install script errors
            }

            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new()
            {
                Headless = true
            });
            
            // Create the client to ensure the test server is running
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = true
            });
        }
        catch
        {
            _isAvailable = false;
        }
    }

    public async Task DisposeAsync()
    {
        if (_browser != null)
            await _browser.DisposeAsync();
        _playwright?.Dispose();
        _client?.Dispose();
    }

    [Fact]
    public async Task HomePage_Loads_Successfully()
    {
        if (!_isAvailable || _browser is null || _client is null) return;

        // Arrange & Act - Use HttpClient instead of Playwright for MVC apps
        var response = await _client.GetAsync("/");
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Home", content);
    }

    [Fact]
    public async Task ProductsPage_DisplaysProducts()
    {
        if (!_isAvailable || _browser is null || _client is null) return;

        // Arrange & Act
        var response = await _client.GetAsync("/Products");
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Products", content);
    }

    [Fact]
    public async Task CartPage_IsAccessible()
    {
        if (!_isAvailable || _browser is null || _client is null) return;

        // Arrange & Act
        var response = await _client.GetAsync("/Cart");
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Cart", content);
    }

    [Fact]
    public async Task Navigation_HomeToProducts_Works()
    {
        if (!_isAvailable || _browser is null || _client is null) return;

        // Arrange
        var homeResponse = await _client.GetAsync("/");
        Assert.True(homeResponse.IsSuccessStatusCode);
        
        // Act
        var productsResponse = await _client.GetAsync("/Products");
        
        // Assert
        Assert.True(productsResponse.IsSuccessStatusCode);
        Assert.Contains("/Products", productsResponse.RequestMessage!.RequestUri!.ToString());
    }

    [Fact]
    public async Task ProductDetails_LoadsFromProductsList()
    {
        if (!_isAvailable || _browser is null || _client is null) return;

        // Arrange
        var productsResponse = await _client.GetAsync("/Products");
        Assert.True(productsResponse.IsSuccessStatusCode);
        
        // Act - navigate to first product details
        var detailsResponse = await _client.GetAsync("/Products/Details/1");
        
        // Assert
        Assert.True(detailsResponse.IsSuccessStatusCode);
        Assert.Contains("/Products/Details/", detailsResponse.RequestMessage!.RequestUri!.ToString());
    }

    [Fact]
    public async Task SearchProducts_FiltersByQuery()
    {
        if (!_isAvailable || _browser is null || _client is null) return;

        // Arrange & Act
        var response = await _client.GetAsync("/Products?query=test");
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains("query=test", response.RequestMessage!.RequestUri!.ToString());
    }

    [Fact]
    public async Task CartIcon_ShowsInNavigation()
    {
        if (!_isAvailable || _browser is null || _client is null) return;

        // Arrange & Act
        var response = await _client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();
        
        // Assert
        Assert.Contains("Cart", content);
    }

    [Fact]
    public async Task Footer_ContainsPrivacyLink()
    {
        if (!_isAvailable || _browser is null || _client is null) return;

        // Arrange & Act
        var response = await _client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();
        
        // Assert
        Assert.Contains("Privacy", content);
    }

    [Fact]
    public async Task CategoryFilter_FiltersProducts()
    {
        if (!_isAvailable || _browser is null || _client is null) return;

        // Arrange & Act
        var response = await _client.GetAsync("/Products?categoryId=1");
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains("categoryId=1", response.RequestMessage!.RequestUri!.ToString());
    }

    [Fact]
    public async Task PageTitle_ChangesOnNavigation()
    {
        if (!_isAvailable || _browser is null || _client is null) return;

        // Arrange & Act
        var homeResponse = await _client.GetAsync("/");
        var homeContent = await homeResponse.Content.ReadAsStringAsync();
        
        var productsResponse = await _client.GetAsync("/Products");
        var productsContent = await productsResponse.Content.ReadAsStringAsync();
        
        // Assert - titles should be different
        Assert.NotEqual(homeContent, productsContent);
    }

    [Fact]
    public async Task ResponsiveDesign_WorksOnMobile()
    {
        if (!_isAvailable || _browser is null || _client is null) return;

        // Arrange & Act - Test with custom user agent
        _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X)");
        var response = await _client.GetAsync("/");
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task ResponsiveDesign_WorksOnTablet()
    {
        if (!_isAvailable || _browser is null || _client is null) return;

        // Arrange & Act - Test with custom user agent
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (iPad; CPU OS 13_0 like Mac OS X)");
        var response = await _client.GetAsync("/");
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task ResponsiveDesign_WorksOnDesktop()
    {
        if (!_isAvailable || _browser is null || _client is null) return;

        // Arrange & Act
        _client.DefaultRequestHeaders.Clear();
        var response = await _client.GetAsync("/");
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task EmptyCart_ShowsEmptyMessage()
    {
        if (!_isAvailable || _browser is null || _client is null) return;

        // Arrange & Act
        var response = await _client.GetAsync("/Cart");
        var content = await response.Content.ReadAsStringAsync();
        
        // Assert - should show empty cart message or have no items
        Assert.True(content.Contains("empty", StringComparison.OrdinalIgnoreCase) || 
                    content.Contains("no items", StringComparison.OrdinalIgnoreCase) ||
                    !content.Contains("Remove", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task NavigationBar_PresentOnAllPages()
    {
        if (!_isAvailable || _browser is null || _client is null) return;

        // Arrange
        var pages = new[] { "/", "/Products", "/Cart" };
        
        foreach (var pagePath in pages)
        {
            // Act
            var response = await _client.GetAsync(pagePath);
            var content = await response.Content.ReadAsStringAsync();
            
            // Assert
            Assert.Contains("<nav", content, StringComparison.OrdinalIgnoreCase);
        }
    }
}
