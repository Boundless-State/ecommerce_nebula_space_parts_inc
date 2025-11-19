using Models.Entities;

namespace Services.Interfaces;

public interface ICartService
{
    Task AddAsync(int productId, int quantity = 1, CancellationToken ct = default);
    Task UpdateQuantityAsync(int productId, int quantity, CancellationToken ct = default);
    Task RemoveAsync(int productId, CancellationToken ct = default);
    Task ClearAsync(CancellationToken ct = default);

    Task<IReadOnlyList<CartItemDto>> GetItemsAsync(CancellationToken ct = default);
    Task<int> GetItemCountAsync(CancellationToken ct = default);
    Task<decimal> GetTotalAsync(CancellationToken ct = default);
}

public record CartItemDto(int ProductId, string Name, decimal UnitPrice, int Quantity, string ImageUrl)
{
    public decimal Subtotal => UnitPrice * Quantity;
}
