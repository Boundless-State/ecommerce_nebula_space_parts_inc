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

public class CheckoutControllerBranchTests
{
    [Fact]
    public async Task Index_When_No_Items_RedirectsAndSetsTempData()
    {
        // Arrange
        var cart = new Mock<ICartService>();
        cart.Setup(c => c.GetItemsAsync(default)).ReturnsAsync(new List<CartItemDto>());
        var pay = new Mock<IPaymentService>();
        var db = new Data.WebshopDbContext(new DbContextOptionsBuilder<Data.WebshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var ctl = new CheckoutController(cart.Object, pay.Object, NullLogger<CheckoutController>.Instance, db)
        { TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>()) };
        // Act
        var res = await ctl.Index();
        // Assert
        var rd = Assert.IsType<RedirectToActionResult>(res);
        Assert.Equal("Index", rd.ActionName);
    }

    [Fact]
    public async Task PlaceOrder_When_Payment_Fails_SetsTempData_And_Redirects()
    {
        // Arrange
        var cart = new Mock<ICartService>();
        cart.Setup(c => c.GetItemsAsync(default)).ReturnsAsync(new List<CartItemDto> { new(1, "A", 10, 1, "u") });
        var pay = new Mock<IPaymentService>();
        pay.Setup(p => p.ChargeAsync(It.IsAny<decimal>(), It.IsAny<PaymentRequest>(), default))
            .ReturnsAsync(new PaymentResult(false, null, "fail"));
        var db = new Data.WebshopDbContext(new DbContextOptionsBuilder<Data.WebshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var ctl = new CheckoutController(cart.Object, pay.Object, NullLogger<CheckoutController>.Instance, db)
        { TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>()) };
        // Act
        var res = await ctl.PlaceOrder("n","e","a");
        // Assert
        var rd = Assert.IsType<RedirectToActionResult>(res);
        Assert.Equal("Index", rd.ActionName);
    }
}
