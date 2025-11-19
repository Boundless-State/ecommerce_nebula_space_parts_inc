using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Models.Entities;
using Services.Implementations;
using Services.Interfaces;
using Xunit;

namespace UnitTests;

/// <summary>
/// Branch coverage tests for ProductService CRUD operations
/// Covers DeleteAsync return true/false branches and other conditionals
/// </summary>
public class ProductService_CrudBranches_Tests
{
    private static WebshopDbContext CreateDb()
    {
        var opts = new DbContextOptionsBuilder<WebshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new WebshopDbContext(opts);
    }

    // GetByIdAsync branches: product exists and IsActive = true
    [Fact]
    public async Task GetByIdAsync_ActiveProduct_ReturnsProduct()
    {
        // Arrange
        using var db = CreateDb();
        var cat = new Category { Id = 1, Name = "C" };
        var product = new Product { Id = 10, Name = "Active", CategoryId = 1, Price = 10, Stock = 5, ImageUrl = "u", IsActive = true, Category = cat };
        db.Categories.Add(cat);
        db.Products.Add(product);
        await db.SaveChangesAsync();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act - IsActive = true branch
        var result = await svc.GetByIdAsync(10);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Active", result.Name);
    }

    // GetByIdAsync branches: product exists but IsActive = false
    [Fact]
    public async Task GetByIdAsync_InactiveProduct_ReturnsNull()
    {
        // Arrange
        using var db = CreateDb();
        var cat = new Category { Id = 1, Name = "C" };
        var product = new Product { Id = 10, Name = "Inactive", CategoryId = 1, Price = 10, Stock = 5, ImageUrl = "u", IsActive = false, Category = cat };
        db.Categories.Add(cat);
        db.Products.Add(product);
        await db.SaveChangesAsync();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act - IsActive = false branch
        var result = await svc.GetByIdAsync(10);
        
        // Assert
        Assert.Null(result);
    }

    // GetByIdAsync branches: product doesn't exist
    [Fact]
    public async Task GetByIdAsync_ProductNotFound_ReturnsNull()
    {
        // Arrange
        using var db = CreateDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act - product not found branch
        var result = await svc.GetByIdAsync(999);
        
        // Assert
        Assert.Null(result);
    }

    // DeleteAsync branches: entity exists (returns true)
    [Fact]
    public async Task DeleteAsync_ExistingProduct_ReturnsTrue()
    {
        // Arrange
        using var db = CreateDb();
        var cat = new Category { Id = 1, Name = "C" };
        var product = new Product { Id = 10, Name = "ToDelete", CategoryId = 1, Price = 10, Stock = 5, ImageUrl = "u", Category = cat };
        db.Categories.Add(cat);
        db.Products.Add(product);
        await db.SaveChangesAsync();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act - entity exists branch (returns true)
        var result = await svc.DeleteAsync(10);
        
        // Assert
        Assert.True(result);
    }

    // DeleteAsync branches: entity doesn't exist (returns false)
    [Fact]
    public async Task DeleteAsync_NonExistingProduct_ReturnsFalse()
    {
        // Arrange
        using var db = CreateDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act - entity is null branch (returns false)
        var result = await svc.DeleteAsync(999);
        
        // Assert
        Assert.False(result);
    }

    // DeleteAsync: verify actual deletion from database
    [Fact]
    public async Task DeleteAsync_RemovesProductFromDatabase()
    {
        // Arrange
        using var db = CreateDb();
        var cat = new Category { Id = 1, Name = "C" };
        var product = new Product { Id = 10, Name = "ToDelete", CategoryId = 1, Price = 10, Stock = 5, ImageUrl = "u", Category = cat };
        db.Categories.Add(cat);
        db.Products.Add(product);
        await db.SaveChangesAsync();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        await svc.DeleteAsync(10);
        
        // Assert
        var deleted = await db.Products.FindAsync(10);
        Assert.Null(deleted);
    }

    // GetAllAsync: filters by IsActive = true
    [Fact]
    public async Task GetAllAsync_ExcludesInactiveProducts()
    {
        // Arrange
        using var db = CreateDb();
        var cat = new Category { Id = 1, Name = "C" };
        db.Categories.Add(cat);
        db.Products.AddRange(
            new Product { Id = 1, Name = "Active1", CategoryId = 1, Price = 10, Stock = 5, ImageUrl = "u", IsActive = true, Category = cat },
            new Product { Id = 2, Name = "Active2", CategoryId = 1, Price = 10, Stock = 5, ImageUrl = "u", IsActive = true, Category = cat },
            new Product { Id = 3, Name = "Inactive", CategoryId = 1, Price = 10, Stock = 5, ImageUrl = "u", IsActive = false, Category = cat }
        );
        await db.SaveChangesAsync();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.GetAllAsync();
        
        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, p => Assert.True(p.IsActive));
    }

    // GetFeaturedAsync: orders by CreatedAt descending
    [Fact]
    public async Task GetFeaturedAsync_OrdersByCreatedAtDescending()
    {
        // Arrange
        using var db = CreateDb();
        var cat = new Category { Id = 1, Name = "C" };
        db.Categories.Add(cat);
        db.Products.AddRange(
            new Product { Id = 1, Name = "Oldest", CategoryId = 1, Price = 10, Stock = 5, ImageUrl = "u", IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-10), Category = cat },
            new Product { Id = 2, Name = "Newest", CategoryId = 1, Price = 10, Stock = 5, ImageUrl = "u", IsActive = true, CreatedAt = DateTime.UtcNow, Category = cat },
            new Product { Id = 3, Name = "Middle", CategoryId = 1, Price = 10, Stock = 5, ImageUrl = "u", IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-5), Category = cat }
        );
        await db.SaveChangesAsync();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.GetFeaturedAsync(10);
        
        // Assert
        Assert.Equal("Newest", result[0].Name);
        Assert.Equal("Middle", result[1].Name);
        Assert.Equal("Oldest", result[2].Name);
    }

    // GetFeaturedAsync: respects count parameter
    [Fact]
    public async Task GetFeaturedAsync_RespectsCountParameter()
    {
        // Arrange
        using var db = CreateDb();
        var cat = new Category { Id = 1, Name = "C" };
        db.Categories.Add(cat);
        for (int i = 1; i <= 20; i++)
        {
            db.Products.Add(new Product 
            { 
                Id = i, 
                Name = $"Product{i}", 
                CategoryId = 1, 
                Price = 10, 
                Stock = 5, 
                ImageUrl = "u", 
                IsActive = true, 
                CreatedAt = DateTime.UtcNow.AddDays(-i),
                Category = cat 
            });
        }
        await db.SaveChangesAsync();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.GetFeaturedAsync(5);
        
        // Assert
        Assert.Equal(5, result.Count);
    }

    // CreateAsync: product is added and returned with ID
    [Fact]
    public async Task CreateAsync_ReturnsProductWithGeneratedId()
    {
        // Arrange
        using var db = CreateDb();
        var cat = new Category { Id = 1, Name = "C" };
        db.Categories.Add(cat);
        await db.SaveChangesAsync();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        var newProduct = new Product { Name = "New", CategoryId = 1, Price = 99, Stock = 10, ImageUrl = "u" };
        
        // Act
        var result = await svc.CreateAsync(newProduct);
        
        // Assert
        Assert.True(result.Id > 0);
        Assert.Equal("New", result.Name);
    }

    // UpdateAsync: product is modified
    [Fact]
    public async Task UpdateAsync_ModifiesExistingProduct()
    {
        // Arrange
        using var db = CreateDb();
        var cat = new Category { Id = 1, Name = "C" };
        var product = new Product { Id = 10, Name = "Old", CategoryId = 1, Price = 10, Stock = 5, ImageUrl = "u", Category = cat };
        db.Categories.Add(cat);
        db.Products.Add(product);
        await db.SaveChangesAsync();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        product.Name = "Updated";
        product.Price = 99;
        await svc.UpdateAsync(product);
        
        // Assert
        var updated = await db.Products.FindAsync(10);
        Assert.Equal("Updated", updated!.Name);
        Assert.Equal(99, updated.Price);
    }

    // GetCategoriesAsync: returns ordered by Name
    [Fact]
    public async Task GetCategoriesAsync_OrdersByName()
    {
        // Arrange
        using var db = CreateDb();
        db.Categories.AddRange(
            new Category { Id = 1, Name = "Zebra" },
            new Category { Id = 2, Name = "Alpha" },
            new Category { Id = 3, Name = "Middle" }
        );
        await db.SaveChangesAsync();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.GetCategoriesAsync();
        
        // Assert
        Assert.Equal("Alpha", result[0].Name);
        Assert.Equal("Middle", result[1].Name);
        Assert.Equal("Zebra", result[2].Name);
    }
}
