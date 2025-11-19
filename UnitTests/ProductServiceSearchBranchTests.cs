using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Models.Entities;
using Services.Implementations;
using Services.Interfaces;
using Xunit;

namespace UnitTests;

public class ProductServiceSearchBranchTests
{
    private static WebshopDbContext CreateDb()
    {
        var opts = new DbContextOptionsBuilder<WebshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new WebshopDbContext(opts);
        db.Categories.AddRange(new Category { Id = 1, Name = "Cat1" }, new Category { Id = 2, Name = "Cat2" });
        db.Products.AddRange(
            new Product { Id = 1, Name = "Photon Drive", Description = "even", CategoryId = 1, Price = 10, Stock = 1, ImageUrl = "u", IsActive = true },
            new Product { Id = 2, Name = "Quantum Coil", Description = "odd", CategoryId = 2, Price = 20, Stock = 1, ImageUrl = "u", IsActive = true }
        );
        db.SaveChanges();
        return db;
    }

    [Fact]
    public async Task Search_With_Query_And_Category_Applies_Both()
    {
        // Arrange
        using var db = CreateDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        // Act
        var res = await svc.SearchAsync("Quantum", 2);
        // Assert
        Assert.Single(res);
    }

    [Fact]
    public async Task Search_With_Whitespace_Query_Ignored_Returns_All_Active()
    {
        // Arrange
        using var db = CreateDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        // Act
        var res = await svc.SearchAsync("   ", null);
        // Assert
        Assert.Equal(2, res.Count);
    }

    [Fact]
    public async Task Search_With_Whitespace_Query_And_Category_Filters_By_Category_Only()
    {
        // Arrange
        using var db = CreateDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        // Act
        var res = await svc.SearchAsync("   ", 1);
        // Assert
        Assert.Single(res);
    }

    [Fact]
    public async Task Search_No_Query_No_Category_Returns_All_Active()
    {
        // Arrange
        using var db = CreateDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        // Act
        var res = await svc.SearchAsync(null, null);
        // Assert
        Assert.Equal(2, res.Count);
    }
}
