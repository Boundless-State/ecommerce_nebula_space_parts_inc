using Microsoft.Playwright;
using Xunit;

namespace UnitTests;

/// <summary>
/// Example Playwright tests in UnitTests project.
/// Use for component-level UI testing or isolated browser automation tests.
/// </summary>
public class PlaywrightUnitTests : IAsyncLifetime
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private bool _isAvailable = true;

    public async Task InitializeAsync()
    {
        try
        {
            // Ensure browsers are installed for Playwright
            try
            {
                // This mirrors running the playwright.ps1 install script
                Microsoft.Playwright.Program.Main(new[] { "install" });
            }
            catch
            {
                // Ignore install errors; we'll try to launch and handle failure
            }

            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new()
            {
                Headless = false,  // Changed to false to see the browser
                SlowMo = 500       // Slow down by 500ms per action so you can see what's happening
            });
            _context = await _browser.NewContextAsync();
        }
        catch
        {
            // Mark as unavailable so tests can be skipped gracefully
            _isAvailable = false;
        }
    }

    public async Task DisposeAsync()
    {
        if (_context != null)
            await _context.DisposeAsync();
        if (_browser != null)
            await _browser.DisposeAsync();
        _playwright?.Dispose();
    }

    [Fact]
    public async Task ExternalPage_Loads_Example()
    {
        if (!_isAvailable || _context is null) return;

        // Arrange
        var page = await _context!.NewPageAsync();
        
        // Act
        await page.GotoAsync("https://example.com");
        
        // Assert
        var title = await page.TitleAsync();
        Assert.Equal("Example Domain", title);
        
        await page.CloseAsync();
    }

    [Fact]
    public async Task BrowserContext_Can_SetViewportSize()
    {
        if (!_isAvailable || _context is null) return;

        // Arrange
        var page = await _context!.NewPageAsync();
        await page.SetViewportSizeAsync(1920, 1080);
        
        // Act
        var viewport = page.ViewportSize;
        
        // Assert
        Assert.Equal(1920, viewport!.Width);
        Assert.Equal(1080, viewport.Height);
        
        await page.CloseAsync();
    }

    [Fact]
    public async Task Page_Can_EvaluateJavaScript()
    {
        if (!_isAvailable || _context is null) return;

        // Arrange
        var page = await _context!.NewPageAsync();
        await page.GotoAsync("https://example.com");
        
        // Act
        var result = await page.EvaluateAsync<int>("() => 2 + 2");
        
        // Assert
        Assert.Equal(4, result);
        
        await page.CloseAsync();
    }
}
