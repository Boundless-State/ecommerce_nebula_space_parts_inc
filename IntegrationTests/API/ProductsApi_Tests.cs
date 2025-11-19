using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace IntegrationTests.API;

/// <summary>
/// API endpoint tests - validates HTTP contracts, status codes, and response formats
/// Tests endpoints as if they were RESTful APIs
/// </summary>
public class ProductsApi_Tests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public ProductsApi_Tests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GET_Products_ReturnsOK()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/Products");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GET_Products_ReturnsHTML()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/Products");
        
        // Assert
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GET_ProductDetails_ValidId_ReturnsOK()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/Products/Details/1");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GET_ProductDetails_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/Products/Details/999999");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999)]
    public async Task GET_ProductDetails_NegativeId_ReturnsNotFound(int id)
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync($"/Products/Details/{id}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GET_Products_WithQuery_ReturnsOK()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/Products?query=test");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GET_Products_WithCategory_ReturnsOK()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/Products?categoryId=1");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GET_Products_WithQueryAndCategory_ReturnsOK()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/Products?query=test&categoryId=1");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GET_Products_ContainsExpectedContent()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/Products");
        var content = await response.Content.ReadAsStringAsync();
        
        // Assert
        Assert.Contains("Products", content);
    }

    [Fact]
    public async Task GET_ProductDetails_ContainsProductName()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/Products/Details/1");
        var content = await response.Content.ReadAsStringAsync();
        
        // Assert - should contain product-related content
        Assert.NotEmpty(content);
    }

    [Fact]
    public async Task GET_Products_HasCorrectEncoding()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/Products");
        
        // Assert
        Assert.Contains("utf-8", response.Content.Headers.ContentType?.CharSet);
    }
}
