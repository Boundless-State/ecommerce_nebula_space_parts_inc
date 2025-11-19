using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace WebshopAI.Tests.Controllers;

public class ProductsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProductsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(_ => { });
    }

    [Fact]
    public async Task Get_Products_Index_Returns_OK()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/Products");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("Products", html);
    }
}
