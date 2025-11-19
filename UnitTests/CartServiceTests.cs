using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.Implementations;
using Services.Interfaces;
using Xunit;

namespace UnitTests;

public class CartService_AAATests
{
    private static (ICartService cart, WebshopDbContext db, IServiceProvider sp) Create()
    {
        var services = new ServiceCollection();
        services.AddDbContext<WebshopDbContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddHttpContextAccessor();
        services.AddScoped<ICartService, CartService>();
        var sp = services.BuildServiceProvider();

        var ctx = new DefaultHttpContext();
        var session = new TestSession();
        ctx.Features.Set<ISessionFeature>(new SessionFeature { Session = session });
        var accessor = sp.GetRequiredService<IHttpContextAccessor>();
        accessor.HttpContext = ctx;

        var db = sp.GetRequiredService<WebshopDbContext>();
        db.Categories.Add(new Models.Entities.Category { Id = 1, Name = "C" });
        db.Products.Add(new Models.Entities.Product { Id = 10, Name = "P", CategoryId = 1, Price = 5, Stock = 9, ImageUrl = "u" });
        db.SaveChanges();

        var cart = sp.GetRequiredService<ICartService>();
        return (cart, db, sp);
    }

    [Fact]
    public async Task Add_Increases_ItemCount()
    {
        // Arrange
        var (cart, _, _) = Create();
        // Act
        await cart.AddAsync(10, 2);
        var count = await cart.GetItemCountAsync();
        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task Add_Increases_Total()
    {
        // Arrange
        var (cart, _, _) = Create();
        // Act
        await cart.AddAsync(10, 2);
        var total = await cart.GetTotalAsync();
        // Assert
        Assert.Equal(10, total);
    }

    [Fact]
    public async Task Update_Changes_ItemCount()
    {
        // Arrange
        var (cart, _, _) = Create();
        await cart.AddAsync(10, 2);
        // Act
        await cart.UpdateQuantityAsync(10, 5);
        var count = await cart.GetItemCountAsync();
        // Assert
        Assert.Equal(5, count);
    }

    [Fact]
    public async Task Update_Changes_Total()
    {
        // Arrange
        var (cart, _, _) = Create();
        await cart.AddAsync(10, 2);
        // Act
        await cart.UpdateQuantityAsync(10, 5);
        var total = await cart.GetTotalAsync();
        // Assert
        Assert.Equal(25, total);
    }

    [Fact]
    public async Task Remove_Empties_ItemCount()
    {
        // Arrange
        var (cart, _, _) = Create();
        await cart.AddAsync(10, 2);
        await cart.UpdateQuantityAsync(10, 5);
        // Act
        await cart.RemoveAsync(10);
        var count = await cart.GetItemCountAsync();
        // Assert
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task Remove_Empties_Total()
    {
        // Arrange
        var (cart, _, _) = Create();
        await cart.AddAsync(10, 2);
        await cart.UpdateQuantityAsync(10, 5);
        // Act
        await cart.RemoveAsync(10);
        var total = await cart.GetTotalAsync();
        // Assert
        Assert.Equal(0, total);
    }

    [Fact]
    public async Task Update_To_Zero_Removes_Item()
    {
        // Arrange
        var (cart, _, _) = Create();
        await cart.AddAsync(10, 3);
        // Act
        await cart.UpdateQuantityAsync(10, 0);
        var items = await cart.GetItemsAsync();
        // Assert
        Assert.Empty(items);
    }

    [Fact]
    public async Task Clear_Empties_Cart()
    {
        // Arrange
        var (cart, _, _) = Create();
        await cart.AddAsync(10, 1);
        // Act
        await cart.ClearAsync();
        var count = await cart.GetItemCountAsync();
        // Assert
        Assert.Equal(0, count);
    }

    private sealed class SessionFeature : ISessionFeature
    {
        public ISession Session { get; set; } = null!;
    }

    private sealed class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _store = new();
        public IEnumerable<string> Keys => _store.Keys;
        public string Id { get; } = Guid.NewGuid().ToString();
        public bool IsAvailable { get; } = true;
        public void Clear() => _store.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _store.Remove(key);
        public void Set(string key, byte[] value) => _store[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _store.TryGetValue(key, out value!);
    }
}
