using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Models.Entities;
using Services.Implementations;
using Services.Interfaces;
using Xunit;

namespace UnitTests;

public class ProductService_CRUD_AAATests
{
    private static WebshopDbContext CreateDb()
    {
        var opts = new DbContextOptionsBuilder<WebshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new WebshopDbContext(opts);
        // Don't call EnsureCreated as it triggers seed data
        return db;
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllActiveProducts()
    {
        // Arrange
        using var db = CreateDb();
        var cat = new Category { Id = 100, Name = "TestCat", CreatedAt = DateTime.UtcNow };
        db.Categories.Add(cat);
        db.Products.AddRange(
            new Product { Id = 101, Name = "A", CategoryId = 100, Price = 10, Stock = 5, ImageUrl = "u", IsActive = true, Category = cat },
            new Product { Id = 102, Name = "B", CategoryId = 100, Price = 20, Stock = 5, ImageUrl = "u", IsActive = true, Category = cat },
            new Product { Id = 103, Name = "C", CategoryId = 100, Price = 30, Stock = 5, ImageUrl = "u", IsActive = false, Category = cat }
        );
        db.SaveChanges();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.GetAllAsync();
        
        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoActiveProducts()
    {
        // Arrange
        using var db = CreateDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.GetAllAsync();
        
        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateAsync_AddsProduct()
    {
        // Arrange
        using var db = CreateDb();
        var cat = new Category { Id = 200, Name = "TestCat", CreatedAt = DateTime.UtcNow };
        db.Categories.Add(cat);
        db.SaveChanges();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        var product = new Product { Name = "New", CategoryId = 200, Price = 99, Stock = 1, ImageUrl = "u" };
        
        // Act
        var result = await svc.CreateAsync(product);
        
        // Assert
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task CreateAsync_SavesProductToDatabase()
    {
        // Arrange
        using var db = CreateDb();
        var cat = new Category { Id = 201, Name = "TestCat", CreatedAt = DateTime.UtcNow };
        db.Categories.Add(cat);
        db.SaveChanges();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        var product = new Product { Name = "New", CategoryId = 201, Price = 99, Stock = 1, ImageUrl = "u" };
        
        // Act
        await svc.CreateAsync(product);
        
        // Assert
        Assert.Equal(1, await db.Products.CountAsync());
    }

    [Fact]
    public async Task UpdateAsync_ModifiesProduct()
    {
        // Arrange
        using var db = CreateDb();
        var cat = new Category { Id = 202, Name = "TestCat", CreatedAt = DateTime.UtcNow };
        var product = new Product { Name = "Old", CategoryId = 202, Price = 10, Stock = 5, ImageUrl = "u", Category = cat };
        db.Categories.Add(cat);
        db.Products.Add(product);
        db.SaveChanges();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        product.Name = "New";
        product.Price = 20;
        await svc.UpdateAsync(product);
        
        // Assert
        var updated = await db.Products.FindAsync(product.Id);
        Assert.Equal("New", updated?.Name);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesPrice()
    {
        // Arrange
        using var db = CreateDb();
        var cat = new Category { Id = 203, Name = "TestCat", CreatedAt = DateTime.UtcNow };
        var product = new Product { Name = "Test", CategoryId = 203, Price = 10, Stock = 5, ImageUrl = "u", Category = cat };
        db.Categories.Add(cat);
        db.Products.Add(product);
        db.SaveChanges();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        product.Price = 99.99m;
        await svc.UpdateAsync(product);
        
        // Assert
        var updated = await db.Products.FindAsync(product.Id);
        Assert.Equal(99.99m, updated?.Price);
    }

    [Fact]
    public async Task DeleteAsync_RemovesProduct()
    {
        // Arrange
        using var db = CreateDb();
        var cat = new Category { Id = 204, Name = "TestCat", CreatedAt = DateTime.UtcNow };
        var product = new Product { Name = "ToDelete", CategoryId = 204, Price = 10, Stock = 5, ImageUrl = "u", Category = cat };
        db.Categories.Add(cat);
        db.Products.Add(product);
        db.SaveChanges();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.DeleteAsync(product.Id);
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenProductNotFound()
    {
        // Arrange
        using var db = CreateDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.DeleteAsync(999);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_RemovesFromDatabase()
    {
        // Arrange
        using var db = CreateDb();
        var cat = new Category { Id = 205, Name = "TestCat", CreatedAt = DateTime.UtcNow };
        var product = new Product { Name = "ToDelete", CategoryId = 205, Price = 10, Stock = 5, ImageUrl = "u", Category = cat };
        db.Categories.Add(cat);
        db.Products.Add(product);
        db.SaveChanges();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        await svc.DeleteAsync(product.Id);
        
        // Assert
        Assert.Equal(0, await db.Products.CountAsync());
    }

    [Fact]
    public async Task GetCategoriesAsync_ReturnsAllCategories()
    {
        // Arrange
        using var db = CreateDb();
        db.Categories.AddRange(
            new Category { Id = 206, Name = "Cat1", CreatedAt = DateTime.UtcNow },
            new Category { Id = 207, Name = "Cat2", CreatedAt = DateTime.UtcNow }
        );
        db.SaveChanges();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.GetCategoriesAsync();
        
        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetCategoriesAsync_ReturnsOrderedByName()
    {
        // Arrange
        using var db = CreateDb();
        db.Categories.AddRange(
            new Category { Id = 208, Name = "Zebra", CreatedAt = DateTime.UtcNow },
            new Category { Id = 209, Name = "Alpha", CreatedAt = DateTime.UtcNow }
        );
        db.SaveChanges();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.GetCategoriesAsync();
        
        // Assert
        Assert.Equal("Alpha", result[0].Name);
    }

    [Fact]
    public async Task GetFeaturedAsync_ReturnsRequestedCount()
    {
        // Arrange
        using var db = CreateDb();
        var cat = new Category { Id = 210, Name = "TestCat", CreatedAt = DateTime.UtcNow };
        db.Categories.Add(cat);
        for (int i = 1; i <= 20; i++)
        {
            db.Products.Add(new Product { Name = $"P{i}", CategoryId = 210, Price = 10, Stock = 5, ImageUrl = "u", IsActive = true, Category = cat });
        }
        db.SaveChanges();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.GetFeaturedAsync(5);
        
        // Assert
        Assert.Equal(5, result.Count);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsProduct_WhenExists()
    {
        // Arrange
        using var db = CreateDb();
        var cat = new Category { Id = 211, Name = "TestCat", CreatedAt = DateTime.UtcNow };
        var product = new Product { Name = "Test", CategoryId = 211, Price = 10, Stock = 5, ImageUrl = "u", IsActive = true, Category = cat };
        db.Categories.Add(cat);
        db.Products.Add(product);
        db.SaveChanges();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.GetByIdAsync(product.Id);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotActive()
    {
        // Arrange
        using var db = CreateDb();
        var cat = new Category { Id = 212, Name = "TestCat", CreatedAt = DateTime.UtcNow };
        var product = new Product { Name = "Test", CategoryId = 212, Price = 10, Stock = 5, ImageUrl = "u", IsActive = false, Category = cat };
        db.Categories.Add(cat);
        db.Products.Add(product);
        db.SaveChanges();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.GetByIdAsync(product.Id);
        
        // Assert
        Assert.Null(result);
    }
}
