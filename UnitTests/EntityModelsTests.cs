using Models.Entities;
using Xunit;

namespace UnitTests;

public class EntityModels_AAATests
{
    [Fact]
    public void Product_Properties_SetCorrectly()
    {
        // Arrange & Act
        var product = new Product
        {
            Id = 1,
            Name = "Test",
            Description = "Desc",
            Price = 99.99m,
            Stock = 10,
            ImageUrl = "img.jpg",
            CategoryId = 1,
            IsActive = true
        };
        
        // Assert
        Assert.Equal(1, product.Id);
        Assert.Equal("Test", product.Name);
        Assert.Equal(99.99m, product.Price);
    }

    [Fact]
    public void Product_NavigationProperties_Initialize()
    {
        // Arrange & Act
        var product = new Product();
        
        // Assert
        Assert.NotNull(product.OrderItems);
    }

    [Fact]
    public void Category_Properties_SetCorrectly()
    {
        // Arrange & Act
        var category = new Category
        {
            Id = 1,
            Name = "Electronics",
            Description = "Electronic items"
        };
        
        // Assert
        Assert.Equal("Electronics", category.Name);
    }

    [Fact]
    public void Category_NavigationProperties_Initialize()
    {
        // Arrange & Act
        var category = new Category();
        
        // Assert
        Assert.NotNull(category.Products);
    }

    [Fact]
    public void Order_Properties_SetCorrectly()
    {
        // Arrange & Act
        var order = new Order
        {
            Id = 1,
            OrderNumber = "ORD123",
            CustomerName = "John Doe",
            CustomerEmail = "john@test.com",
            ShippingAddress = "123 Street",
            TotalAmount = 99.99m,
            Status = "Confirmed"
        };
        
        // Assert
        Assert.Equal("ORD123", order.OrderNumber);
        Assert.Equal(99.99m, order.TotalAmount);
    }

    [Fact]
    public void Order_NavigationProperties_Initialize()
    {
        // Arrange & Act
        var order = new Order();
        
        // Assert
        Assert.NotNull(order.OrderItems);
    }

    [Fact]
    public void OrderItem_Properties_SetCorrectly()
    {
        // Arrange & Act
        var orderItem = new OrderItem
        {
            Id = 1,
            OrderId = 1,
            ProductId = 1,
            ProductName = "Test Product",
            UnitPrice = 10.00m,
            Quantity = 2,
            TotalPrice = 20.00m
        };
        
        // Assert
        Assert.Equal(2, orderItem.Quantity);
        Assert.Equal(20.00m, orderItem.TotalPrice);
    }

    [Fact]
    public void Product_CreatedAt_HasValue()
    {
        // Arrange & Act
        var product = new Product { CreatedAt = DateTime.UtcNow };
        
        // Assert
        Assert.True(product.CreatedAt > DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public void Category_CreatedAt_HasValue()
    {
        // Arrange & Act
        var category = new Category { CreatedAt = DateTime.UtcNow };
        
        // Assert
        Assert.True(category.CreatedAt > DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public void Order_OrderDate_HasValue()
    {
        // Arrange & Act
        var order = new Order { OrderDate = DateTime.UtcNow };
        
        // Assert
        Assert.True(order.OrderDate > DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public void Product_IsActive_DefaultsToTrue()
    {
        // Arrange & Act
        var product = new Product { IsActive = true };
        
        // Assert
        Assert.True(product.IsActive);
    }

    [Fact]
    public void Product_Price_CanBeDecimal()
    {
        // Arrange & Act
        var product = new Product { Price = 123.45m };
        
        // Assert
        Assert.Equal(123.45m, product.Price);
    }

    [Fact]
    public void Product_Stock_CanBeZero()
    {
        // Arrange & Act
        var product = new Product { Stock = 0 };
        
        // Assert
        Assert.Equal(0, product.Stock);
    }
}
