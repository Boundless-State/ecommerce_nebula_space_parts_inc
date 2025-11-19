using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models.Entities;
using Services.Implementations;
using Services.Interfaces;
using Xunit;

namespace UnitTests;

/// <summary>
/// Branch coverage tests for CartService - covers all conditional paths
/// </summary>
public class CartService_AllBranches_Tests
{
    private const string CartKey = "CART_V1";

    private static (ICartService cart, WebshopDbContext db, TestSession session) CreateService()
    {
        var services = new ServiceCollection();
        services.AddDbContext<WebshopDbContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddHttpContextAccessor();
        services.AddScoped<ICartService, CartService>();
        var sp = services.BuildServiceProvider();

        var ctx = new DefaultHttpContext();
        var session = new TestSession();
        ctx.Features.Set<ISessionFeature>(new SessionFeature { Session = session });
        sp.GetRequiredService<IHttpContextAccessor>().HttpContext = ctx;

        var db = sp.GetRequiredService<WebshopDbContext>();
        db.Categories.Add(new Category { Id = 1, Name = "Test" });
        db.Products.AddRange(
            new Product { Id = 10, Name = "P1", CategoryId = 1, Price = 10, Stock = 50, ImageUrl = "u1" },
            new Product { Id = 11, Name = "P2", CategoryId = 1, Price = 20, Stock = 50, ImageUrl = "u2" }
        );
        db.SaveChanges();

        return (sp.GetRequiredService<ICartService>(), db, session);
    }

    // AddAsync branches: existing item path
    [Fact]
    public async Task AddAsync_ExistingItem_IncreasesQuantity()
    {
        // Arrange
        var (cart, _, _) = CreateService();
        await cart.AddAsync(10, 2);
        
        // Act - add again to trigger existing item branch
        await cart.AddAsync(10, 3);
        
        // Assert
        var items = await cart.GetItemsAsync();
        Assert.Single(items);
        Assert.Equal(5, items[0].Quantity);
    }

    // AddAsync branches: new item path
    [Fact]
    public async Task AddAsync_NewItem_AddsToCart()
    {
        // Arrange
        var (cart, _, _) = CreateService();
        
        // Act - new item branch
        await cart.AddAsync(10, 2);
        
        // Assert
        var items = await cart.GetItemsAsync();
        Assert.Single(items);
        Assert.Equal(2, items[0].Quantity);
    }

    // AddAsync branches: quantity <= 0 clamps to 1 (new item)
    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(-100)]
    public async Task AddAsync_NonPositiveQuantity_NewItem_ClampsToOne(int qty)
    {
        // Arrange
        var (cart, _, _) = CreateService();
        
        // Act - non-positive quantity branch (new item)
        await cart.AddAsync(10, qty);
        
        // Assert
        var count = await cart.GetItemCountAsync();
        Assert.Equal(1, count);
    }

    // AddAsync branches: quantity <= 0 clamps to 1 (existing item)
    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public async Task AddAsync_NonPositiveQuantity_ExistingItem_ClampsToOne(int qty)
    {
        // Arrange
        var (cart, _, _) = CreateService();
        await cart.AddAsync(10, 5);
        
        // Act - add with non-positive quantity to existing item
        await cart.AddAsync(10, qty);
        
        // Assert
        var count = await cart.GetItemCountAsync();
        Assert.Equal(6, count); // 5 + 1
    }

    // UpdateQuantityAsync branches: quantity <= 0 removes item
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-50)]
    public async Task UpdateQuantityAsync_NonPositive_RemovesItem(int qty)
    {
        // Arrange
        var (cart, _, _) = CreateService();
        await cart.AddAsync(10, 5);
        
        // Act - quantity <= 0 branch
        await cart.UpdateQuantityAsync(10, qty);
        
        // Assert
        var items = await cart.GetItemsAsync();
        Assert.Empty(items);
    }

    // UpdateQuantityAsync branches: quantity > 0 and item exists
    [Fact]
    public async Task UpdateQuantityAsync_PositiveQuantity_ExistingItem_UpdatesQuantity()
    {
        // Arrange
        var (cart, _, _) = CreateService();
        await cart.AddAsync(10, 2);
        
        // Act - positive quantity, existing item branch
        await cart.UpdateQuantityAsync(10, 10);
        
        // Assert
        var items = await cart.GetItemsAsync();
        Assert.Single(items);
        Assert.Equal(10, items[0].Quantity);
    }

    // UpdateQuantityAsync branches: quantity > 0 but item doesn't exist
    [Fact]
    public async Task UpdateQuantityAsync_PositiveQuantity_NonExistingItem_NoChange()
    {
        // Arrange
        var (cart, _, _) = CreateService();
        
        // Act - item doesn't exist branch
        await cart.UpdateQuantityAsync(999, 5);
        
        // Assert
        var items = await cart.GetItemsAsync();
        Assert.Empty(items);
    }

    // GetCartInternalAsync branches: empty session
    [Fact]
    public async Task GetItemsAsync_EmptySession_ReturnsEmpty()
    {
        // Arrange
        var (cart, _, _) = CreateService();
        
        // Act - empty session branch
        var items = await cart.GetItemsAsync();
        
        // Assert
        Assert.Empty(items);
    }

    // GetCartInternalAsync branches: invalid JSON (try-catch)
    [Fact]
    public async Task GetItemsAsync_InvalidJson_ReturnsEmpty()
    {
        // Arrange
        var (cart, _, session) = CreateService();
        session.SetString(CartKey, "{invalid json");
        
        // Act - catch branch for invalid JSON
        var items = await cart.GetItemsAsync();
        
        // Assert
        Assert.Empty(items);
    }

    // GetCartInternalAsync branches: null JSON deserialization result
    [Fact]
    public async Task GetItemsAsync_NullDeserialization_ReturnsEmpty()
    {
        // Arrange
        var (cart, _, session) = CreateService();
        session.SetString(CartKey, "null");
        
        // Act - null deserialization branch
        var items = await cart.GetItemsAsync();
        
        // Assert
        Assert.Empty(items);
    }

    // RemoveAsync branches: item exists
    [Fact]
    public async Task RemoveAsync_ExistingItem_RemovesFromCart()
    {
        // Arrange
        var (cart, _, _) = CreateService();
        await cart.AddAsync(10, 2);
        await cart.AddAsync(11, 3);
        
        // Act - remove existing item
        await cart.RemoveAsync(10);
        
        // Assert
        var items = await cart.GetItemsAsync();
        Assert.Single(items);
        Assert.Equal(11, items[0].ProductId);
    }

    // RemoveAsync branches: item doesn't exist (no-op)
    [Fact]
    public async Task RemoveAsync_NonExistingItem_NoChange()
    {
        // Arrange
        var (cart, _, _) = CreateService();
        await cart.AddAsync(10, 2);
        
        // Act - remove non-existing item (no-op branch)
        await cart.RemoveAsync(999);
        
        // Assert
        var items = await cart.GetItemsAsync();
        Assert.Single(items);
    }

    // ClearAsync - always takes same path (no branches)
    [Fact]
    public async Task ClearAsync_RemovesAllItems()
    {
        // Arrange
        var (cart, _, _) = CreateService();
        await cart.AddAsync(10, 2);
        await cart.AddAsync(11, 3);
        
        // Act
        await cart.ClearAsync();
        
        // Assert
        var items = await cart.GetItemsAsync();
        Assert.Empty(items);
    }

    // Test multiple products to verify correct item updates
    [Fact]
    public async Task UpdateQuantityAsync_MultipleProducts_UpdatesCorrectItem()
    {
        // Arrange
        var (cart, _, _) = CreateService();
        await cart.AddAsync(10, 5);
        await cart.AddAsync(11, 3);
        
        // Act
        await cart.UpdateQuantityAsync(10, 10);
        
        // Assert
        var items = await cart.GetItemsAsync();
        var item10 = items.First(i => i.ProductId == 10);
        var item11 = items.First(i => i.ProductId == 11);
        Assert.Equal(10, item10.Quantity);
        Assert.Equal(3, item11.Quantity); // unchanged
    }

    private sealed class SessionFeature : ISessionFeature { public ISession Session { get; set; } = null!; }
    public sealed class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _s = new();
        public IEnumerable<string> Keys => _s.Keys;
        public string Id { get; } = Guid.NewGuid().ToString();
        public bool IsAvailable => true;
        public void Clear() => _s.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _s.Remove(key);
        public void Set(string key, byte[] value) => _s[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _s.TryGetValue(key, out value!);
        public void SetString(string key, string value) => _s[key] = System.Text.Encoding.UTF8.GetBytes(value);
    }
}
