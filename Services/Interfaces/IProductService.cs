using Models.Entities;

namespace Services.Interfaces;

public interface IProductService
{
    Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Product>> GetFeaturedAsync(int count = 10, CancellationToken cancellationToken = default);
    Task<List<Product>> SearchAsync(string? query, int? categoryId, CancellationToken cancellationToken = default);
    Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default);
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default);
}
