using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Services.Interfaces;
using webshopAI.Controllers;
using Xunit;

namespace UnitTests;

/// <summary>
/// Comprehensive branch coverage for all controller actions
/// Covers success and error paths
/// </summary>
public class Controllers_AllBranches_Tests
{
    // CheckoutController.Index - empty cart branch
    [Fact]
    public async Task CheckoutIndex_EmptyCart_Redirects()
    {
        // Arrange
        var cartMock = new Mock<ICartService>();
        cartMock.Setup(c => c.GetItemsAsync(default)).ReturnsAsync(new List<CartItemDto>());
        var payMock = new Mock<IPaymentService>();
        var db = new Data.WebshopDbContext(new DbContextOptionsBuilder<Data.WebshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var controller = new CheckoutController(cartMock.Object, payMock.Object, NullLogger<CheckoutController>.Instance, db)
        {
            TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
        };
        
        // Act - empty cart branch
        var result = await controller.Index();
        
        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Cart", redirect.ControllerName);
    }

    // CheckoutController.Index - has items branch
    [Fact]
    public async Task CheckoutIndex_WithItems_ReturnsView()
    {
        // Arrange
        var cartMock = new Mock<ICartService>();
        cartMock.Setup(c => c.GetItemsAsync(default)).ReturnsAsync(new List<CartItemDto> 
        { 
            new(1, "Product", 10, 1, "u") 
        });
        var payMock = new Mock<IPaymentService>();
        var db = new Data.WebshopDbContext(new DbContextOptionsBuilder<Data.WebshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var controller = new CheckoutController(cartMock.Object, payMock.Object, NullLogger<CheckoutController>.Instance, db)
        {
            TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
        };
        
        // Act - has items branch
        var result = await controller.Index();
        
        // Assert
        Assert.IsType<ViewResult>(result);
    }

    // CheckoutController.PlaceOrder - empty cart branch
    [Fact]
    public async Task PlaceOrder_EmptyCart_Redirects()
    {
        // Arrange
        var cartMock = new Mock<ICartService>();
        cartMock.Setup(c => c.GetItemsAsync(default)).ReturnsAsync(new List<CartItemDto>());
        var payMock = new Mock<IPaymentService>();
        var db = new Data.WebshopDbContext(new DbContextOptionsBuilder<Data.WebshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var controller = new CheckoutController(cartMock.Object, payMock.Object, NullLogger<CheckoutController>.Instance, db)
        {
            TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
        };
        
        // Act - empty cart branch
        var result = await controller.PlaceOrder("Name", "email@test.com", "Address");
        
        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Cart", redirect.ControllerName);
    }

    // CheckoutController.PlaceOrder - payment failed branch
    [Fact]
    public async Task PlaceOrder_PaymentFailed_RedirectsToCheckout()
    {
        // Arrange
        var cartMock = new Mock<ICartService>();
        cartMock.Setup(c => c.GetItemsAsync(default)).ReturnsAsync(new List<CartItemDto> 
        { 
            new(1, "Product", 10, 1, "u") 
        });
        var payMock = new Mock<IPaymentService>();
        payMock.Setup(p => p.ChargeAsync(It.IsAny<decimal>(), It.IsAny<PaymentRequest>(), default))
            .ReturnsAsync(new PaymentResult(false, null, "Payment declined"));
        var db = new Data.WebshopDbContext(new DbContextOptionsBuilder<Data.WebshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var controller = new CheckoutController(cartMock.Object, payMock.Object, NullLogger<CheckoutController>.Instance, db)
        {
            TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
        };
        
        // Act - payment failed branch
        var result = await controller.PlaceOrder("Name", "email@test.com", "Address");
        
        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Contains("Payment failed", controller.TempData["Err"]?.ToString());
    }

    // CheckoutController.PlaceOrder - success branch
    [Fact]
    public async Task PlaceOrder_Success_RedirectsToConfirmation()
    {
        // Arrange
        var cartMock = new Mock<ICartService>();
        cartMock.Setup(c => c.GetItemsAsync(default)).ReturnsAsync(new List<CartItemDto> 
        { 
            new(1, "Product", 10, 2, "u") 
        });
        var payMock = new Mock<IPaymentService>();
        payMock.Setup(p => p.ChargeAsync(It.IsAny<decimal>(), It.IsAny<PaymentRequest>(), default))
            .ReturnsAsync(new PaymentResult(true, "TXN123"));
        var db = new Data.WebshopDbContext(new DbContextOptionsBuilder<Data.WebshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var controller = new CheckoutController(cartMock.Object, payMock.Object, NullLogger<CheckoutController>.Instance, db)
        {
            TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
        };
        
        // Act - success branch
        var result = await controller.PlaceOrder("Name", "email@test.com", "Address");
        
        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Confirmation", redirect.ActionName);
    }

    // CheckoutController.Confirmation - order found branch
    [Fact]
    public async Task Confirmation_OrderFound_ReturnsView()
    {
        // Arrange
        var cartMock = new Mock<ICartService>();
        var payMock = new Mock<IPaymentService>();
        var db = new Data.WebshopDbContext(new DbContextOptionsBuilder<Data.WebshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var order = new Models.Entities.Order 
        { 
            OrderNumber = "ORD123", 
            CustomerName = "Test", 
            CustomerEmail = "test@test.com", 
            ShippingAddress = "123", 
            TotalAmount = 100, 
            Status = "Confirmed" 
        };
        db.Orders.Add(order);
        await db.SaveChangesAsync();
        var controller = new CheckoutController(cartMock.Object, payMock.Object, NullLogger<CheckoutController>.Instance, db);
        
        // Act - order found branch
        var result = await controller.Confirmation(order.Id);
        
        // Assert
        Assert.IsType<ViewResult>(result);
    }

    // CheckoutController.Confirmation - order not found branch
    [Fact]
    public async Task Confirmation_OrderNotFound_ReturnsNotFound()
    {
        // Arrange
        var cartMock = new Mock<ICartService>();
        var payMock = new Mock<IPaymentService>();
        var db = new Data.WebshopDbContext(new DbContextOptionsBuilder<Data.WebshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var controller = new CheckoutController(cartMock.Object, payMock.Object, NullLogger<CheckoutController>.Instance, db);
        
        // Act - order not found branch
        var result = await controller.Confirmation(999);
        
        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    // ProductsController.Details - product found branch
    [Fact]
    public async Task ProductDetails_Found_ReturnsView()
    {
        // Arrange
        var svcMock = new Mock<IProductService>();
        svcMock.Setup(s => s.GetByIdAsync(1, default))
            .ReturnsAsync(new Models.Entities.Product 
            { 
                Id = 1, 
                Name = "Test", 
                Category = new Models.Entities.Category { Name = "Cat" } 
            });
        var controller = new webshopAI.Controllers.ProductsController(
            svcMock.Object, 
            NullLogger<webshopAI.Controllers.ProductsController>.Instance);
        
        // Act - product found branch
        var result = await controller.Details(1);
        
        // Assert
        Assert.IsType<ViewResult>(result);
    }

    // ProductsController.Details - product not found branch
    [Fact]
    public async Task ProductDetails_NotFound_ReturnsNotFound()
    {
        // Arrange
        var svcMock = new Mock<IProductService>();
        svcMock.Setup(s => s.GetByIdAsync(999, default)).ReturnsAsync((Models.Entities.Product?)null);
        var controller = new webshopAI.Controllers.ProductsController(
            svcMock.Object, 
            NullLogger<webshopAI.Controllers.ProductsController>.Instance);
        
        // Act - product not found branch
        var result = await controller.Details(999);
        
        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
