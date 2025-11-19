using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Models.Entities;
using Services.Implementations;
using Services.Interfaces;
using Xunit;

namespace UnitTests;

public class CheckoutFlow_AAATests
{
    private static WebshopDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<WebshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new WebshopDbContext(options);
        db.Categories.Add(new Category { Id = 1, Name = "Cat" });
        db.Products.AddRange(
            new Product { Id = 1, Name = "A", CategoryId = 1, Price = 10, Stock = 5, ImageUrl = "u" },
            new Product { Id = 2, Name = "B", CategoryId = 1, Price = 20, Stock = 5, ImageUrl = "u" }
        );
        db.SaveChanges();
        return db;
    }

    [Fact]
    public void Cart_Total_Computed_Correctly()
    {
        // Arrange
        var items = new List<CartItemDto> { new(1, "A", 10, 2, "u"), new(2, "B", 20, 1, "u") };
        // Act
        var amount = items.Sum(i => i.UnitPrice * i.Quantity);
        // Assert
        Assert.Equal(40, amount);
    }

    public static IEnumerable<object[]> Amounts => Enumerable.Range(1, 50).Select(i => new object[] { (decimal)(i * 3.5m) });

    [Theory]
    [MemberData(nameof(Amounts))]
    public async Task Payment_Succeeds_For_Amount(decimal amount)
    {
        // Arrange
        var provider = new MockPaymentProvider(new NullLogger<MockPaymentProvider>());
        var req = new PaymentRequest { CustomerName = "X", CustomerEmail = "x@y" };
        // Act
        var res = await provider.ChargeAsync(amount, req);
        // Assert
        Assert.True(res.Success);
    }

    [Fact]
    public async Task Payment_TransactionId_NotEmpty()
    {
        // Arrange
        var provider = new MockPaymentProvider(new NullLogger<MockPaymentProvider>());
        // Act
        var res = await provider.ChargeAsync(10, new PaymentRequest { CustomerName = "X", CustomerEmail = "x@y" });
        // Assert
        Assert.False(string.IsNullOrWhiteSpace(res.TransactionId));
    }
}
