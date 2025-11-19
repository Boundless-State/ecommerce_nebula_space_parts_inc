using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Services.Interfaces;

namespace Services.Implementations;

public class ProductService : IProductService
{
    private readonly WebshopDbContext _db;
    private readonly ILogger<ProductService> _logger;

    public ProductService(WebshopDbContext db, ILogger<ProductService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Products.AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _db.Products.AsNoTracking()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive, cancellationToken);
    }

    public async Task<List<Product>> GetFeaturedAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        return await _db.Products.AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Product>> SearchAsync(string? query, int? categoryId, CancellationToken cancellationToken = default)
    {
        var q = _db.Products.AsNoTracking().Include(p => p.Category).Where(p => p.IsActive);
        if (!string.IsNullOrWhiteSpace(query))
        {
            var t = query.Trim();
            q = q.Where(p => EF.Functions.Like(p.Name, $"%{t}%") || EF.Functions.Like(p.Description, $"%{t}%"));
        }
        if (categoryId.HasValue)
        {
            q = q.Where(p => p.CategoryId == categoryId.Value);
        }
        return await q.OrderBy(p => p.Name).ToListAsync(cancellationToken);
    }

    public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _db.Products.Update(product);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Products.FindAsync(new object[] { id }, cancellationToken);
        if (entity is null) return false;
        _db.Products.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<List<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(cancellationToken);
    }
}
