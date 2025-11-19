using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace IntegrationTests;

public class Smoke_AAATests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public Smoke_AAATests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task Products_Index_OK()
    {
        // Arrange
        var client = _factory.CreateClient();
        // Act
        var res = await client.GetAsync("/Products");
        // Assert
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Checkout_Empty_Redirects()
    {
        // Arrange
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        // Act
        var res = await client.GetAsync("/Checkout");
        // Assert
        Assert.Equal(HttpStatusCode.Redirect, res.StatusCode);
    }
}
