using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.Implementations;
using Services.Interfaces;
using Xunit;

namespace UnitTests;

public class CartServiceBranchesJsonTests
{
    private const string CartKey = "CART_V1";

    private static (ICartService cart, TestSession session) Create()
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
        return (sp.GetRequiredService<ICartService>(), session);
    }

    [Fact]
    public async Task GetItems_EmptySession_Returns_Empty()
    {
        // Arrange
        var (cart, _) = Create();
        // Act
        var items = await cart.GetItemsAsync();
        // Assert
        Assert.Empty(items);
    }

    [Fact]
    public async Task GetItems_InvalidJson_Returns_Empty()
    {
        // Arrange
        var (cart, session) = Create();
        session.SetString(CartKey, "{\"bad\": true"); // malformed
        // Act
        var items = await cart.GetItemsAsync();
        // Assert
        Assert.Empty(items);
    }

    private sealed class SessionFeature : ISessionFeature { public ISession Session { get; set; } = null!; }
    public sealed class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _s = new();
        public IEnumerable<string> Keys => _s.Keys; public string Id { get; } = Guid.NewGuid().ToString(); public bool IsAvailable => true;
        public void Clear() => _s.Clear(); public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask; public void Remove(string key) => _s.Remove(key);
        public void Set(string key, byte[] value) => _s[key] = value; public bool TryGetValue(string key, out byte[] value) => _s.TryGetValue(key, out value!);
        public void SetString(string key, string value) => _s[key] = System.Text.Encoding.UTF8.GetBytes(value);
    }
}
