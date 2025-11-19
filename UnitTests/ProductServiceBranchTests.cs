using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Models.Entities;
using Services.Implementations;
using Services.Interfaces;
using Xunit;

namespace UnitTests;

public class ProductServiceBranchTests
{
    private static WebshopDbContext CreateDb()
    {
        var opts = new DbContextOptionsBuilder<WebshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new WebshopDbContext(opts);
        db.Categories.Add(new Category { Id = 1, Name = "C" });
        db.Products.AddRange(
            new Product { Id = 1, Name = "Even", Description = "even", CategoryId = 1, Price = 10, Stock = 1, ImageUrl = "u", IsActive = true },
            new Product { Id = 2, Name = "Odd", Description = "odd", CategoryId = 1, Price = 20, Stock = 1, ImageUrl = "u", IsActive = true }
        );
        db.SaveChanges();
        return db;
    }

    [Fact]
    public async Task Search_Name_Filter_Branch()
    {
        // Arrange
        using var db = CreateDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        // Act
        var res = await svc.SearchAsync("Odd", null);
        // Assert
        Assert.Single(res);
    }

    [Fact]
    public async Task Search_Category_Filter_Branch()
    {
        // Arrange
        using var db = CreateDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        // Act
        var res = await svc.SearchAsync(null, 1);
        // Assert
        Assert.Equal(2, res.Count);
    }
}
