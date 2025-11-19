using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Models.Entities;
using Services.Implementations;
using Services.Interfaces;
using Xunit;

namespace UnitTests;

/// <summary>
/// Comprehensive branch coverage tests for ProductService.SearchAsync
/// Tests all conditional branches: query null, whitespace, with value, category null, with value
/// </summary>
public class ProductService_SearchBranches_Tests
{
    private static WebshopDbContext CreateTestDb()
    {
        var opts = new DbContextOptionsBuilder<WebshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new WebshopDbContext(opts);
        
        db.Categories.AddRange(
            new Category { Id = 1, Name = "Propulsion" },
            new Category { Id = 2, Name = "Navigation" }
        );
        
        db.Products.AddRange(
            new Product { Id = 1, Name = "Ion Drive", Description = "Fast propulsion", CategoryId = 1, Price = 100, Stock = 5, ImageUrl = "i1", IsActive = true },
            new Product { Id = 2, Name = "Warp Core", Description = "FTL travel", CategoryId = 1, Price = 500, Stock = 2, ImageUrl = "i2", IsActive = true },
            new Product { Id = 3, Name = "Star Chart", Description = "Navigation maps", CategoryId = 2, Price = 50, Stock = 10, ImageUrl = "i3", IsActive = true },
            new Product { Id = 4, Name = "Compass", Description = "Basic navigation", CategoryId = 2, Price = 10, Stock = 20, ImageUrl = "i4", IsActive = true },
            new Product { Id = 5, Name = "Inactive", Description = "Not shown", CategoryId = 1, Price = 1, Stock = 0, ImageUrl = "i5", IsActive = false }
        );
        
        db.SaveChanges();
        return db;
    }

    [Fact]
    public async Task Search_NullQuery_NullCategory_ReturnsAllActive()
    {
        // Arrange
        using var db = CreateTestDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.SearchAsync(null, null);
        
        // Assert
        Assert.Equal(4, result.Count); // Excludes inactive
    }

    [Fact]
    public async Task Search_EmptyQuery_NullCategory_ReturnsAllActive()
    {
        // Arrange
        using var db = CreateTestDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.SearchAsync("", null);
        
        // Assert
        Assert.Equal(4, result.Count);
    }

    [Fact]
    public async Task Search_WhitespaceQuery_NullCategory_ReturnsAllActive()
    {
        // Arrange
        using var db = CreateTestDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.SearchAsync("   ", null);
        
        // Assert
        Assert.Equal(4, result.Count);
    }

    [Fact]
    public async Task Search_ValidQuery_NullCategory_FiltersByName()
    {
        // Arrange
        using var db = CreateTestDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.SearchAsync("Drive", null);
        
        // Assert
        Assert.Single(result);
        Assert.Equal("Ion Drive", result[0].Name);
    }

    [Fact]
    public async Task Search_ValidQuery_NullCategory_FiltersByDescription()
    {
        // Arrange
        using var db = CreateTestDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.SearchAsync("navigation", null);
        
        // Assert
        Assert.Equal(2, result.Count); // Star Chart and Compass
    }

    [Fact]
    public async Task Search_NullQuery_ValidCategory_FiltersByCategory()
    {
        // Arrange
        using var db = CreateTestDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.SearchAsync(null, 1);
        
        // Assert
        Assert.Equal(2, result.Count); // Ion Drive, Warp Core (excludes inactive)
    }

    [Fact]
    public async Task Search_EmptyQuery_ValidCategory_FiltersByCategory()
    {
        // Arrange
        using var db = CreateTestDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.SearchAsync("", 2);
        
        // Assert
        Assert.Equal(2, result.Count); // Star Chart, Compass
    }

    [Fact]
    public async Task Search_ValidQuery_ValidCategory_AppliesBothFilters()
    {
        // Arrange
        using var db = CreateTestDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.SearchAsync("Core", 1);
        
        // Assert
        Assert.Single(result);
        Assert.Equal("Warp Core", result[0].Name);
    }

    [Fact]
    public async Task Search_ValidQuery_ValidCategory_NoMatch_ReturnsEmpty()
    {
        // Arrange
        using var db = CreateTestDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.SearchAsync("NonExistent", 1);
        
        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Search_CaseInsensitive_Query_MatchesRegardlessOfCase()
    {
        // Arrange
        using var db = CreateTestDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var resultLower = await svc.SearchAsync("drive", null);
        var resultUpper = await svc.SearchAsync("DRIVE", null);
        var resultMixed = await svc.SearchAsync("DrIvE", null);
        
        // Assert
        Assert.Single(resultLower);
        Assert.Single(resultUpper);
        Assert.Single(resultMixed);
    }

    [Fact]
    public async Task Search_QueryWithLeadingTrailingSpaces_TrimsAndMatches()
    {
        // Arrange
        using var db = CreateTestDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.SearchAsync("  Drive  ", null);
        
        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task Search_InvalidCategory_ReturnsEmpty()
    {
        // Arrange
        using var db = CreateTestDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.SearchAsync(null, 999);
        
        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Search_Results_OrderedByName()
    {
        // Arrange
        using var db = CreateTestDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.SearchAsync(null, null);
        
        // Assert
        Assert.Equal("Compass", result[0].Name);
        Assert.Equal("Ion Drive", result[1].Name);
        Assert.Equal("Star Chart", result[2].Name);
        Assert.Equal("Warp Core", result[3].Name);
    }

    [Fact]
    public async Task Search_IncludesCategory_InResults()
    {
        // Arrange
        using var db = CreateTestDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        
        // Act
        var result = await svc.SearchAsync(null, 1);
        
        // Assert
        Assert.All(result, p => Assert.NotNull(p.Category));
        Assert.All(result, p => Assert.Equal("Propulsion", p.Category.Name));
    }
}
