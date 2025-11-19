using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Models.Entities;
using Moq;
using Services.Implementations;
using Services.Interfaces;
using webshopAI.Controllers;
using Xunit;

namespace UnitTests;

/// <summary>
/// Negative scenario tests - error paths, invalid inputs, edge cases
/// </summary>
public class NegativeScenarios_Tests
{
    // Invalid product ID
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999)]
    public async Task GetByIdAsync_InvalidId_ReturnsNull(int invalidId)
    {
        // Arrange
        var db = new WebshopDbContext(new DbContextOptionsBuilder<WebshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.GetByIdAsync(invalidId);
        
        // Assert
        Assert.Null(result);
    }

    // Cart operations with invalid product
    [Fact]
    public async Task CartAdd_NonExistentProduct_ThrowsException()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDbContext<WebshopDbContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddHttpContextAccessor();
        services.AddScoped<ICartService, CartService>();
        var sp = services.BuildServiceProvider();
        var ctx = new DefaultHttpContext();
        ctx.Features.Set<ISessionFeature>(new TestSessionFeature { Session = new TestSession() });
        sp.GetRequiredService<IHttpContextAccessor>().HttpContext = ctx;
        var cart = sp.GetRequiredService<ICartService>();
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await cart.AddAsync(999, 1)); // Product doesn't exist
    }

    // Empty or null search queries
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task Search_EmptyOrWhitespace_ReturnsAllActiveProducts(string? query)
    {
        // Arrange
        var db = new WebshopDbContext(new DbContextOptionsBuilder<WebshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        db.Categories.Add(new Category { Id = 1, Name = "C" });
        db.Products.Add(new Product { Id = 1, Name = "P", CategoryId = 1, Price = 10, Stock = 5, ImageUrl = "u", IsActive = true });
        await db.SaveChangesAsync();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.SearchAsync(query, null);
        
        // Assert
        Assert.Single(result);
    }

    // Checkout with empty/null customer data
    [Theory]
    [InlineData(null, "email@test.com", "Address")]
    [InlineData("", "email@test.com", "Address")]
    [InlineData("Name", null, "Address")]
    [InlineData("Name", "", "Address")]
    [InlineData("Name", "email@test.com", null)]
    [InlineData("Name", "email@test.com", "")]
    public async Task PlaceOrder_InvalidCustomerData_StillProcesses(string? name, string? email, string? address)
    {
        // Arrange - even with invalid data, order processes (validation should be added in real app)
        var cartMock = new Mock<ICartService>();
        cartMock.Setup(c => c.GetItemsAsync(default)).ReturnsAsync(new List<CartItemDto> 
        { 
            new(1, "Product", 10, 1, "u") 
        });
        var payMock = new Mock<IPaymentService>();
        payMock.Setup(p => p.ChargeAsync(It.IsAny<decimal>(), It.IsAny<PaymentRequest>(), default))
            .ReturnsAsync(new PaymentResult(true, "TXN"));
        var db = new WebshopDbContext(new DbContextOptionsBuilder<WebshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var controller = new CheckoutController(cartMock.Object, payMock.Object, NullLogger<CheckoutController>.Instance, db)
        {
            TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
        };
        
        // Act - tests that system handles invalid input (even if it shouldn't)
        var result = await controller.PlaceOrder(name!, email!, address!);
        
        // Assert - current implementation doesn't validate, so it redirects to confirmation
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Confirmation", redirect.ActionName);
    }

    // Payment with zero or negative amount
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Payment_InvalidAmount_StillSucceeds(decimal amount)
    {
        // Arrange - MockPaymentProvider doesn't validate amount
        var provider = new MockPaymentProvider(NullLogger<MockPaymentProvider>.Instance);
        var request = new PaymentRequest { CustomerName = "Test", CustomerEmail = "test@test.com" };
        
        // Act
        var result = await provider.ChargeAsync(amount, request);
        
        // Assert - current mock implementation always succeeds
        Assert.True(result.Success);
    }

    // Delete already deleted product
    [Fact]
    public async Task DeleteAsync_AlreadyDeleted_ReturnsFalse()
    {
        // Arrange
        var db = new WebshopDbContext(new DbContextOptionsBuilder<WebshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var cat = new Category { Id = 1, Name = "C" };
        var product = new Product { Id = 1, Name = "P", CategoryId = 1, Price = 10, Stock = 5, ImageUrl = "u", Category = cat };
        db.Categories.Add(cat);
        db.Products.Add(product);
        await db.SaveChangesAsync();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act - delete twice
        var firstDelete = await svc.DeleteAsync(1);
        var secondDelete = await svc.DeleteAsync(1);
        
        // Assert
        Assert.True(firstDelete);
        Assert.False(secondDelete);
    }

    // Update non-existent product
    [Fact]
    public async Task UpdateAsync_NonExistentProduct_ThrowsException()
    {
        // Arrange
        var db = new WebshopDbContext(new DbContextOptionsBuilder<WebshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        var product = new Product { Id = 999, Name = "NonExistent", CategoryId = 1, Price = 10, Stock = 5, ImageUrl = "u" };
        
        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () => 
            await svc.UpdateAsync(product));
    }

    // Cart operations on empty cart
    [Fact]
    public async Task UpdateQuantity_EmptyCart_NoEffect()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDbContext<WebshopDbContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddHttpContextAccessor();
        services.AddScoped<ICartService, CartService>();
        var sp = services.BuildServiceProvider();
        var ctx = new DefaultHttpContext();
        ctx.Features.Set<ISessionFeature>(new TestSessionFeature { Session = new TestSession() });
        sp.GetRequiredService<IHttpContextAccessor>().HttpContext = ctx;
        var cart = sp.GetRequiredService<ICartService>();
        
        // Act - update on empty cart
        await cart.UpdateQuantityAsync(999, 5);
        
        // Assert
        var items = await cart.GetItemsAsync();
        Assert.Empty(items);
    }

    [Fact]
    public async Task Remove_EmptyCart_NoEffect()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDbContext<WebshopDbContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddHttpContextAccessor();
        services.AddScoped<ICartService, CartService>();
        var sp = services.BuildServiceProvider();
        var ctx = new DefaultHttpContext();
        ctx.Features.Set<ISessionFeature>(new TestSessionFeature { Session = new TestSession() });
        sp.GetRequiredService<IHttpContextAccessor>().HttpContext = ctx;
        var cart = sp.GetRequiredService<ICartService>();
        
        // Act - remove from empty cart
        await cart.RemoveAsync(999);
        
        // Assert
        var items = await cart.GetItemsAsync();
        Assert.Empty(items);
    }

    // Search with non-existent category
    [Fact]
    public async Task Search_NonExistentCategory_ReturnsEmpty()
    {
        // Arrange
        var db = new WebshopDbContext(new DbContextOptionsBuilder<WebshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.SearchAsync(null, 999);
        
        // Assert
        Assert.Empty(result);
    }

    // Extreme quantities
    [Theory]
    [InlineData(int.MaxValue)]
    [InlineData(1000000)]
    public async Task CartAdd_ExtremeQuantity_Succeeds(int quantity)
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDbContext<WebshopDbContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddHttpContextAccessor();
        services.AddScoped<ICartService, CartService>();
        var sp = services.BuildServiceProvider();
        var ctx = new DefaultHttpContext();
        ctx.Features.Set<ISessionFeature>(new TestSessionFeature { Session = new TestSession() });
        sp.GetRequiredService<IHttpContextAccessor>().HttpContext = ctx;
        var db = sp.GetRequiredService<WebshopDbContext>();
        db.Categories.Add(new Category { Id = 1, Name = "C" });
        db.Products.Add(new Product { Id = 1, Name = "P", CategoryId = 1, Price = 10, Stock = int.MaxValue, ImageUrl = "u" });
        await db.SaveChangesAsync();
        var cart = sp.GetRequiredService<ICartService>();
        
        // Act - extreme quantity
        await cart.AddAsync(1, quantity);
        
        // Assert
        var count = await cart.GetItemCountAsync();
        Assert.Equal(quantity, count);
    }

    // Confirmation with invalid order ID
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(999999)]
    public async Task Confirmation_InvalidOrderId_ReturnsNotFound(int invalidId)
    {
        // Arrange
        var cartMock = new Mock<ICartService>();
        var payMock = new Mock<IPaymentService>();
        var db = new WebshopDbContext(new DbContextOptionsBuilder<WebshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var controller = new CheckoutController(cartMock.Object, payMock.Object, NullLogger<CheckoutController>.Instance, db);
        
        // Act
        var result = await controller.Confirmation(invalidId);
        
        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    private sealed class TestSessionFeature : ISessionFeature { public ISession Session { get; set; } = null!; }
    private sealed class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _s = new();
        public IEnumerable<string> Keys => _s.Keys;
        public string Id { get; } = Guid.NewGuid().ToString();
        public bool IsAvailable => true;
        public void Clear() => _s.Clear();
        public Task CommitAsync(CancellationToken ct = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken ct = default) => Task.CompletedTask;
        public void Remove(string key) => _s.Remove(key);
        public void Set(string key, byte[] value) => _s[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _s.TryGetValue(key, out value!);
    }
}
