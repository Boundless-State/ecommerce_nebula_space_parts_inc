using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Models.Entities;
using Services.Interfaces;
using Xunit;

namespace UnitTests;

public class ProductsController_AAATests
{
    [Fact]
    public async Task Index_Returns_View()
    {
        // Arrange
        var mockSvc = new Mock<IProductService>();
        mockSvc.Setup(s => s.SearchAsync(null, null, default)).ReturnsAsync(new List<Product> { new() { Id = 1, Name = "X", Category = new Category { Name = "C" } } });
        mockSvc.Setup(s => s.GetCategoriesAsync(default)).ReturnsAsync(new List<Category> { new() { Id = 1, Name = "C" } });
        var controller = new webshopAI.Controllers.ProductsController(mockSvc.Object, NullLogger<webshopAI.Controllers.ProductsController>.Instance);
        // Act
        var result = await controller.Index(null, null);
        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Details_NotFound_When_Missing()
    {
        // Arrange
        var mockSvc = new Mock<IProductService>();
        mockSvc.Setup(s => s.GetByIdAsync(123, default)).ReturnsAsync((Product?)null);
        var controller = new webshopAI.Controllers.ProductsController(mockSvc.Object, NullLogger<webshopAI.Controllers.ProductsController>.Instance);
        // Act
        var result = await controller.Details(123);
        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
