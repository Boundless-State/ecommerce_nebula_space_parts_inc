using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.Implementations;
using Services.Interfaces;
using Xunit;

namespace UnitTests;

public class CartServiceBranchTests
{
    private static (ICartService cart, WebshopDbContext db) Create()
    {
        var services = new ServiceCollection();
        services.AddDbContext<WebshopDbContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddHttpContextAccessor();
        services.AddScoped<ICartService, CartService>();
        var sp = services.BuildServiceProvider();
        var ctx = new DefaultHttpContext();
        ctx.Features.Set<ISessionFeature>(new SessionFeature { Session = new TestSession() });
        sp.GetRequiredService<IHttpContextAccessor>().HttpContext = ctx;
        var db = sp.GetRequiredService<WebshopDbContext>();
        db.Categories.Add(new Models.Entities.Category { Id = 1, Name = "C" });
        db.Products.Add(new Models.Entities.Product { Id = 1, Name = "P", CategoryId = 1, Price = 10, Stock = 2, ImageUrl = "u" });
        db.SaveChanges();
        return (sp.GetRequiredService<ICartService>(), db);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Add_Branch_Clamps_NonPositive_To_One(int qty)
    {
        // Arrange
        var (cart, _) = Create();
        // Act
        await cart.AddAsync(1, qty);
        var count = await cart.GetItemCountAsync();
        // Assert
        Assert.Equal(1, count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task Update_Branch_Removes_When_NonPositive(int qty)
    {
        // Arrange
        var (cart, _) = Create();
        await cart.AddAsync(1, 2);
        // Act
        await cart.UpdateQuantityAsync(1, qty);
        var count = await cart.GetItemCountAsync();
        // Assert
        Assert.Equal(0, count);
    }

    private sealed class SessionFeature : ISessionFeature { public ISession Session { get; set; } = null!; }
    private sealed class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _s = new();
        public IEnumerable<string> Keys => _s.Keys; public string Id { get; } = Guid.NewGuid().ToString(); public bool IsAvailable => true;
        public void Clear() => _s.Clear(); public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask; public void Remove(string key) => _s.Remove(key);
        public void Set(string key, byte[] value) => _s[key] = value; public bool TryGetValue(string key, out byte[] value) => _s.TryGetValue(key, out value!);
    }
}
