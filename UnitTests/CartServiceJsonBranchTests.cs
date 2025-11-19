using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.Implementations;
using Services.Interfaces;
using Xunit;

namespace UnitTests;

public class CartServiceJsonBranchTests
{
    private const string CartKey = "CART_V1"; // match CartService internal key

    private static (ICartService cart, ISession session) Create()
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
    public async Task GetItems_Returns_Empty_On_Invalid_Json()
    {
        // Arrange
        var (cart, session) = Create();
        session.SetString(CartKey, "not a json blob");
        // Act
        var items = await cart.GetItemsAsync();
        // Assert
        Assert.Empty(items);
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
