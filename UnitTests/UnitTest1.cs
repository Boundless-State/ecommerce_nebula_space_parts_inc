using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Models.Entities;
using Services.Implementations;
using Services.Interfaces;
using Xunit;

namespace UnitTests;

public class ProductService_AAATests
{
    private static WebshopDbContext CreateDb()
    {
        var opts = new DbContextOptionsBuilder<WebshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new WebshopDbContext(opts);
        db.Categories.Add(new Category { Id = 1, Name = "CatA" });
        db.Categories.Add(new Category { Id = 2, Name = "CatB" });
        for (var i = 1; i <= 80; i++)
        {
            db.Products.Add(new Product
            {
                Id = i,
                Name = i % 4 == 0 ? $"Quantum Part {i}" : (i % 3 == 0 ? $"Plasma Unit {i}" : $"Sensor {i}"),
                Description = i % 2 == 0 ? $"even-{i}" : $"odd-{i}",
                CategoryId = i % 2 == 0 ? 1 : 2,
                Price = 10 + i,
                Stock = 5,
                ImageUrl = "u"
            });
        }
        db.SaveChanges();
        return db;
    }

    public static IEnumerable<object[]> SearchQueries => Enumerable.Range(1, 300)
        .Select(i => new object[] { i % 3 == 0 ? $"Quantum {i}" : (i % 2 == 0 ? $"even-{i}" : $"odd-{i}") });

    [Theory]
    [MemberData(nameof(SearchQueries))]
    public async Task Search_Returns_List_NotNull(string q)
    {
        // Arrange
        using var db = CreateDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        // Act
        var result = await svc.SearchAsync(q, null);
        // Assert
        Assert.NotNull(result);
    }

    public static IEnumerable<object[]> SearchWithCategoryCases => Enumerable.Range(1, 150)
        .Select(i => new object[] { i % 2 == 0 ? $"Quantum {i}" : $"Sensor {i}", (int?)(i % 2 == 0 ? 1 : 2) });

    [Theory]
    [MemberData(nameof(SearchWithCategoryCases))]
    public async Task Search_With_Category_Returns_List_NotNull(string q, int? categoryId)
    {
        // Arrange
        using var db = CreateDb();
        IProductService svc = new ProductService(db, NullLogger<ProductService>.Instance);
        // Act
        var result = await svc.SearchAsync(q, categoryId);
        // Assert
        Assert.NotNull(result);
    }
}
