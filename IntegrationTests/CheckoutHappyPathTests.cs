using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace IntegrationTests;

public class CheckoutHappyPath_AAATests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    public CheckoutHappyPath_AAATests(TestWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task Checkout_Page_Returns_OK_Or_Redirect()
    {
        // Arrange
        var client = _factory.CreateClient();
        // Act
        var res = await client.GetAsync("/Checkout");
        // Assert
        Assert.True(res.StatusCode is HttpStatusCode.OK or HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task Home_Page_Returns_OK()
    {
        // Arrange
        var client = _factory.CreateClient();
        // Act
        var res = await client.GetAsync("/");
        // Assert
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Products_Page_Returns_OK()
    {
        // Arrange
        var client = _factory.CreateClient();
        // Act
        var res = await client.GetAsync("/Products");
        // Assert
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Cart_Page_Returns_OK()
    {
        // Arrange
        var client = _factory.CreateClient();
        // Act
        var res = await client.GetAsync("/Cart");
        // Assert
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Product_Details_Returns_OK_For_Valid_Id()
    {
        // Arrange
        var client = _factory.CreateClient();
        // Act
        var res = await client.GetAsync("/Products/Details/1");
        // Assert
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }
}
