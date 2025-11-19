using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Services.Interfaces;
using System.Text.Json;

namespace Services.Implementations;

public class CartService(IHttpContextAccessor accessor, WebshopDbContext db) : ICartService
{
    private const string CartKey = "CART_V1";
    private readonly IHttpContextAccessor _accessor = accessor;
    private readonly WebshopDbContext _db = db;

    private ISession Session => _accessor.HttpContext?.Session ?? throw new InvalidOperationException("No session available");

    public async Task AddAsync(int productId, int quantity = 1, CancellationToken ct = default)
    {
        var cart = await GetCartInternalAsync(ct);
        var existing = cart.FirstOrDefault(x => x.ProductId == productId);
        if (existing is null)
        {
            var p = await _db.Products.AsNoTracking().FirstAsync(x => x.Id == productId, ct);
            cart.Add(new CartItemDto(p.Id, p.Name, p.Price, Math.Max(1, quantity), p.ImageUrl));
        }
        else
        {
            var newQty = existing.Quantity + Math.Max(1, quantity);
            cart = cart.Select(x => x.ProductId == productId ? existing with { Quantity = newQty } : x).ToList();
        }
        await SaveCartInternalAsync(cart, ct);
    }

    public async Task UpdateQuantityAsync(int productId, int quantity, CancellationToken ct = default)
    {
        var cart = await GetCartInternalAsync(ct);
        if (quantity <= 0)
        {
            cart = cart.Where(x => x.ProductId != productId).ToList();
        }
        else
        {
            var existing = cart.FirstOrDefault(x => x.ProductId == productId);
            if (existing is not null)
            {
                cart = cart.Select(x => x.ProductId == productId ? existing with { Quantity = quantity } : x).ToList();
            }
        }
        await SaveCartInternalAsync(cart, ct);
    }

    public async Task RemoveAsync(int productId, CancellationToken ct = default)
    {
        var cart = await GetCartInternalAsync(ct);
        cart = cart.Where(x => x.ProductId != productId).ToList();
        await SaveCartInternalAsync(cart, ct);
    }

    public async Task ClearAsync(CancellationToken ct = default)
    {
        await SaveCartInternalAsync(new List<CartItemDto>(), ct);
    }

    public Task<IReadOnlyList<CartItemDto>> GetItemsAsync(CancellationToken ct = default)
        => GetCartInternalAsync(ct).ContinueWith(t => (IReadOnlyList<CartItemDto>)t.Result, ct);

    public async Task<int> GetItemCountAsync(CancellationToken ct = default)
        => (await GetCartInternalAsync(ct)).Sum(x => x.Quantity);

    public async Task<decimal> GetTotalAsync(CancellationToken ct = default)
        => (await GetCartInternalAsync(ct)).Sum(x => x.Subtotal);

    private async Task<List<CartItemDto>> GetCartInternalAsync(CancellationToken ct)
    {
        var json = Session.GetString(CartKey);
        if (string.IsNullOrEmpty(json)) return new List<CartItemDto>();
        try
        {
            var items = JsonSerializer.Deserialize<List<CartItemDto>>(json) ?? new List<CartItemDto>();
            return await Task.FromResult(items);
        }
        catch
        {
            return new List<CartItemDto>();
        }
    }

    private Task SaveCartInternalAsync(List<CartItemDto> items, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(items);
        Session.SetString(CartKey, json);
        return Task.CompletedTask;
    }
}
