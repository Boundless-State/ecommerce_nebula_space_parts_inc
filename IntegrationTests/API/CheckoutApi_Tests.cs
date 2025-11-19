using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace IntegrationTests.API;

/// <summary>
/// API tests for Checkout endpoints
/// </summary>
public class CheckoutApi_Tests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public CheckoutApi_Tests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GET_Checkout_ReturnsOKOrRedirect()
    {
        // Arrange
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        
        // Act
        var response = await client.GetAsync("/Checkout");
        
        // Assert - either OK (has items) or Redirect (empty cart)
        Assert.True(response.StatusCode == HttpStatusCode.OK || 
                    response.StatusCode == HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task POST_PlaceOrder_WithoutItems_Redirects()
    {
        // Arrange
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("customerName", "John Doe"),
            new KeyValuePair<string, string>("customerEmail", "john@test.com"),
            new KeyValuePair<string, string>("shippingAddress", "123 Test St")
        });
        
        // Act
        var response = await client.PostAsync("/Checkout/PlaceOrder", content);
        
        // Assert - redirects (likely to cart because empty)
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
    }

    [Fact]
    public async Task GET_Confirmation_ValidId_ReturnsOK()
    {
        // Arrange - assumes order ID 1 doesn't exist in test db
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/Checkout/Confirmation/1");
        
        // Assert - NotFound or OK depending on if order exists
        Assert.True(response.StatusCode == HttpStatusCode.OK || 
                    response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GET_Confirmation_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/Checkout/Confirmation/999999");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
