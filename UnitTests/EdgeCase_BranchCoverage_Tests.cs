using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models.Entities;
using Services.Implementations;
using Services.Interfaces;
using System.Text.Json;
using Xunit;

namespace UnitTests;

/// <summary>
/// Edge case tests to improve branch coverage from 66% to 80%+
/// Tests all conditional branches, error paths, and boundary conditions
/// </summary>
public class EdgeCase_BranchCoverage_Tests
{
    private const string CartKey = "CART_V1";

    private static (ICartService cart, WebshopDbContext db, TestSession session) CreateCartService()
    {
        var services = new ServiceCollection();
        services.AddDbContext<WebshopDbContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddHttpContextAccessor();
        services.AddScoped<ICartService, CartService>();
        var sp = services.BuildServiceProvider();

        var ctx = new DefaultHttpContext();
        var session = new TestSession();
        ctx.Features.Set<ISessionFeature>(new SessionFeature { Session = session });
        sp.GetRequiredService<IHttpContextAccessor>().HttpContext = ctx;

        var db = sp.GetRequiredService<WebshopDbContext>();
        db.Categories.Add(new Category { Id = 1, Name = "Test Category" });
        db.Products.AddRange(
            new Product { Id = 10, Name = "Product A", CategoryId = 1, Price = 10, Stock = 50, ImageUrl = "imgA.jpg", IsActive = true },
            new Product { Id = 11, Name = "Product B", CategoryId = 1, Price = 20, Stock = 50, ImageUrl = "imgB.jpg", IsActive = true },
            new Product { Id = 12, Name = "Inactive Product", CategoryId = 1, Price = 30, Stock = 0, ImageUrl = "imgC.jpg", IsActive = false }
        );
        db.SaveChanges();

        return (sp.GetRequiredService<ICartService>(), db, session);
    }

    #region CartService Edge Cases

    // BRANCH: GetCartInternalAsync - empty session (null json)
    [Fact]
    public async Task CartService_GetItems_EmptySession_ReturnsEmptyList()
    {
        // Arrange
        var (cart, _, _) = CreateCartService();
        
        // Act - session is empty, json is null
        var items = await cart.GetItemsAsync();
        
        // Assert
        Assert.Empty(items);
    }

    // BRANCH: GetCartInternalAsync - corrupted JSON in session (catch block)
    [Fact]
    public async Task CartService_GetItems_CorruptedJson_ReturnsEmptyList()
    {
        // Arrange
        var (cart, _, session) = CreateCartService();
        session.SetString(CartKey, "{ invalid json }");
        
        // Act - triggers catch block in deserialization
        var items = await cart.GetItemsAsync();
        
        // Assert
        Assert.Empty(items);
    }

    // BRANCH: AddAsync - quantity <= 0 for NEW item (Math.Max clamps to 1)
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task CartService_AddAsync_ZeroOrNegativeQuantity_ClampsToOne(int qty)
    {
        // Arrange
        var (cart, _, _) = CreateCartService();
        
        // Act - quantity <= 0 branch for new item
        await cart.AddAsync(10, qty);
        
        // Assert - should clamp to 1
        var items = await cart.GetItemsAsync();
        Assert.Single(items);
        Assert.Equal(1, items[0].Quantity);
    }

    // BRANCH: AddAsync - quantity <= 0 for EXISTING item (Math.Max clamps to 1)
    [Fact]
    public async Task CartService_AddAsync_ExistingItem_NegativeQuantity_ClampsToOne()
    {
        // Arrange
        var (cart, _, _) = CreateCartService();
        await cart.AddAsync(10, 5);
        
        // Act - add with negative quantity (existing item branch)
        await cart.AddAsync(10, -3);
        
        // Assert - should add Math.Max(1, -3) = 1, so total = 6
        var items = await cart.GetItemsAsync();
        Assert.Single(items);
        Assert.Equal(6, items[0].Quantity); // 5 + 1
    }

    // BRANCH: UpdateQuantityAsync - quantity <= 0 (removes item)
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CartService_UpdateQuantity_ZeroOrNegative_RemovesItem(int qty)
    {
        // Arrange
        var (cart, _, _) = CreateCartService();
        await cart.AddAsync(10, 5);
        
        // Act - quantity <= 0 branch
        await cart.UpdateQuantityAsync(10, qty);
        
        // Assert - item should be removed
        var items = await cart.GetItemsAsync();
        Assert.Empty(items);
    }

    // BRANCH: UpdateQuantityAsync - quantity > 0 (updates quantity)
    [Fact]
    public async Task CartService_UpdateQuantity_PositiveValue_UpdatesQuantity()
    {
        // Arrange
        var (cart, _, _) = CreateCartService();
        await cart.AddAsync(10, 5);
        
        // Act - quantity > 0 branch
        await cart.UpdateQuantityAsync(10, 10);
        
        // Assert
        var items = await cart.GetItemsAsync();
        Assert.Single(items);
        Assert.Equal(10, items[0].Quantity);
    }

    // BRANCH: UpdateQuantityAsync - item not found (existing is null)
    [Fact]
    public async Task CartService_UpdateQuantity_NonExistentItem_NoChange()
    {
        // Arrange
        var (cart, _, _) = CreateCartService();
        await cart.AddAsync(10, 5);
        
        // Act - trying to update non-existent item (existing is null branch)
        await cart.UpdateQuantityAsync(999, 10);
        
        // Assert - no change
        var items = await cart.GetItemsAsync();
        Assert.Single(items);
        Assert.Equal(10, items[0].ProductId);
    }

    // BRANCH: RemoveAsync - removes specific item
    [Fact]
    public async Task CartService_Remove_ExistingItem_RemovesOnlyThatItem()
    {
        // Arrange
        var (cart, _, _) = CreateCartService();
        await cart.AddAsync(10, 2);
        await cart.AddAsync(11, 3);
        
        // Act
        await cart.RemoveAsync(10);
        
        // Assert
        var items = await cart.GetItemsAsync();
        Assert.Single(items);
        Assert.Equal(11, items[0].ProductId);
    }

    // BRANCH: RemoveAsync - non-existent item (no error, just filters)
    [Fact]
    public async Task CartService_Remove_NonExistentItem_NoError()
    {
        // Arrange
        var (cart, _, _) = CreateCartService();
        await cart.AddAsync(10, 2);
        
        // Act
        await cart.RemoveAsync(999);
        
        // Assert - original item still there
        var items = await cart.GetItemsAsync();
        Assert.Single(items);
        Assert.Equal(10, items[0].ProductId);
    }

    // BRANCH: ClearAsync - empties cart
    [Fact]
    public async Task CartService_Clear_RemovesAllItems()
    {
        // Arrange
        var (cart, _, _) = CreateCartService();
        await cart.AddAsync(10, 2);
        await cart.AddAsync(11, 3);
        
        // Act
        await cart.ClearAsync();
        
        // Assert
        var items = await cart.GetItemsAsync();
        Assert.Empty(items);
    }

    // BRANCH: GetItemCountAsync - empty cart returns 0
    [Fact]
    public async Task CartService_GetItemCount_EmptyCart_ReturnsZero()
    {
        // Arrange
        var (cart, _, _) = CreateCartService();
        
        // Act
        var count = await cart.GetItemCountAsync();
        
        // Assert
        Assert.Equal(0, count);
    }

    // BRANCH: GetItemCountAsync - sums quantities correctly
    [Fact]
    public async Task CartService_GetItemCount_MultipleItems_SumsQuantities()
    {
        // Arrange
        var (cart, _, _) = CreateCartService();
        await cart.AddAsync(10, 2);
        await cart.AddAsync(11, 3);
        
        // Act
        var count = await cart.GetItemCountAsync();
        
        // Assert
        Assert.Equal(5, count);
    }

    // BRANCH: GetTotalAsync - empty cart returns 0
    [Fact]
    public async Task CartService_GetTotal_EmptyCart_ReturnsZero()
    {
        // Arrange
        var (cart, _, _) = CreateCartService();
        
        // Act
        var total = await cart.GetTotalAsync();
        
        // Assert
        Assert.Equal(0m, total);
    }

    // BRANCH: GetTotalAsync - calculates subtotals correctly
    [Fact]
    public async Task CartService_GetTotal_MultipleItems_CalculatesCorrectly()
    {
        // Arrange
        var (cart, _, _) = CreateCartService();
        await cart.AddAsync(10, 2); // 10 * 2 = 20
        await cart.AddAsync(11, 3); // 20 * 3 = 60
        
        // Act
        var total = await cart.GetTotalAsync();
        
        // Assert
        Assert.Equal(80m, total);
    }

    #endregion

    #region ProductService Edge Cases

    // BRANCH: SearchAsync - null/empty query (skip where clause)
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ProductService_Search_NullOrEmptyQuery_ReturnsAllActiveProducts(string? query)
    {
        // Arrange
        using var db = CreateDbContext();
        var service = new ProductService(db, Microsoft.Extensions.Logging.Abstractions.NullLogger<ProductService>.Instance);
        
        // Act - string.IsNullOrWhiteSpace branch
        var results = await service.SearchAsync(query, null);
        
        // Assert - returns active products only
        Assert.Equal(2, results.Count); // Only active products
        Assert.DoesNotContain(results, p => !p.IsActive);
    }

    // BRANCH: SearchAsync - with query and category
    [Fact]
    public async Task ProductService_Search_WithQueryAndCategory_FiltersCorrectly()
    {
        // Arrange
        using var db = CreateDbContext();
        var service = new ProductService(db, Microsoft.Extensions.Logging.Abstractions.NullLogger<ProductService>.Instance);
        
        // Act - both if branches true
        var results = await service.SearchAsync("Product", 1);
        
        // Assert
        Assert.Equal(2, results.Count);
        Assert.All(results, p => Assert.Contains("Product", p.Name));
    }

    // BRANCH: SearchAsync - query only, no category
    [Fact]
    public async Task ProductService_Search_QueryOnly_FiltersCorrectly()
    {
        // Arrange
        using var db = CreateDbContext();
        var service = new ProductService(db, Microsoft.Extensions.Logging.Abstractions.NullLogger<ProductService>.Instance);
        
        // Act - query branch true, categoryId branch false
        var results = await service.SearchAsync("Product A", null);
        
        // Assert
        Assert.Single(results);
        Assert.Equal("Product A", results[0].Name);
    }

    // BRANCH: SearchAsync - category only, no query
    [Fact]
    public async Task ProductService_Search_CategoryOnly_FiltersCorrectly()
    {
        // Arrange
        using var db = CreateDbContext();
        db.Categories.Add(new Category { Id = 2, Name = "Category 2" });
        db.Products.Add(new Product { Id = 13, Name = "Product C", CategoryId = 2, Price = 15, Stock = 10, ImageUrl = "c.jpg", IsActive = true });
        await db.SaveChangesAsync();
        var service = new ProductService(db, Microsoft.Extensions.Logging.Abstractions.NullLogger<ProductService>.Instance);
        
        // Act - query branch false, categoryId branch true
        var results = await service.SearchAsync(null, 2);
        
        // Assert
        Assert.Single(results);
        Assert.Equal(2, results[0].CategoryId);
    }

    // BRANCH: DeleteAsync - product exists (returns true)
    [Fact]
    public async Task ProductService_Delete_ExistingProduct_ReturnsTrue()
    {
        // Arrange
        using var db = CreateDbContext();
        var service = new ProductService(db, Microsoft.Extensions.Logging.Abstractions.NullLogger<ProductService>.Instance);
        
        // Act - entity is not null branch
        var result = await service.DeleteAsync(10);
        
        // Assert
        Assert.True(result);
        var deleted = await db.Products.FindAsync(10);
        Assert.Null(deleted);
    }

    // BRANCH: DeleteAsync - product doesn't exist (returns false)
    [Fact]
    public async Task ProductService_Delete_NonExistentProduct_ReturnsFalse()
    {
        // Arrange
        using var db = CreateDbContext();
        var service = new ProductService(db, Microsoft.Extensions.Logging.Abstractions.NullLogger<ProductService>.Instance);
        
        // Act - entity is null branch
        var result = await service.DeleteAsync(999);
        
        // Assert
        Assert.False(result);
    }

    // BRANCH: GetByIdAsync - inactive product (returns null)
    [Fact]
    public async Task ProductService_GetById_InactiveProduct_ReturnsNull()
    {
        // Arrange
        using var db = CreateDbContext();
        var service = new ProductService(db, Microsoft.Extensions.Logging.Abstractions.NullLogger<ProductService>.Instance);
        
        // Act - IsActive = false branch
        var result = await service.GetByIdAsync(12);
        
        // Assert
        Assert.Null(result);
    }

    // BRANCH: GetByIdAsync - active product (returns product)
    [Fact]
    public async Task ProductService_GetById_ActiveProduct_ReturnsProduct()
    {
        // Arrange
        using var db = CreateDbContext();
        var service = new ProductService(db, Microsoft.Extensions.Logging.Abstractions.NullLogger<ProductService>.Instance);
        
        // Act - IsActive = true branch
        var result = await service.GetByIdAsync(10);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Product A", result.Name);
    }

    // BRANCH: GetFeaturedAsync - returns limited count
    [Fact]
    public async Task ProductService_GetFeatured_ReturnsCorrectCount()
    {
        // Arrange
        using var db = CreateDbContext();
        var service = new ProductService(db, Microsoft.Extensions.Logging.Abstractions.NullLogger<ProductService>.Instance);
        
        // Act - Take(count) branch
        var results = await service.GetFeaturedAsync(1);
        
        // Assert
        Assert.Single(results);
    }

    #endregion

    #region Controller Edge Cases

    // Test Cart Controller with different quantities
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    public async Task CartController_Add_VariousQuantities_HandlesCorrectly(int qty)
    {
        // Arrange
        var (cart, db, session) = CreateCartService();
        var controller = new webshopAI.Controllers.CartController(
            cart, 
            new ProductService(db, Microsoft.Extensions.Logging.Abstractions.NullLogger<ProductService>.Instance),
            Microsoft.Extensions.Logging.Abstractions.NullLogger<webshopAI.Controllers.CartController>.Instance
        );
        
        // Act
        var result = await controller.Add(10, qty);
        
        // Assert
        Assert.IsType<Microsoft.AspNetCore.Mvc.RedirectToActionResult>(result);
        var items = await cart.GetItemsAsync();
        Assert.Single(items);
        Assert.True(items[0].Quantity >= 1); // Should be at least 1 due to Math.Max
    }

    #endregion

    #region Helper Methods

    private static WebshopDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<WebshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new WebshopDbContext(options);
        
        db.Categories.Add(new Category { Id = 1, Name = "Test Category" });
        db.Products.AddRange(
            new Product { Id = 10, Name = "Product A", CategoryId = 1, Price = 10, Stock = 50, ImageUrl = "imgA.jpg", IsActive = true, Description = "Description A" },
            new Product { Id = 11, Name = "Product B", CategoryId = 1, Price = 20, Stock = 50, ImageUrl = "imgB.jpg", IsActive = true, Description = "Description B" },
            new Product { Id = 12, Name = "Inactive Product", CategoryId = 1, Price = 30, Stock = 0, ImageUrl = "imgC.jpg", IsActive = false, Description = "Inactive" }
        );
        db.SaveChanges();
        
        return db;
    }

    #endregion
}
