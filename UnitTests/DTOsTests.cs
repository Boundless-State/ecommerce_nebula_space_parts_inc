using Services.Interfaces;
using Xunit;

namespace UnitTests;

public class DTOs_AAATests
{
    [Fact]
    public void CartItemDto_Subtotal_Calculated()
    {
        // Arrange
        var dto = new CartItemDto(1, "Product", 10.50m, 3, "img.jpg");
        
        // Act
        var subtotal = dto.Subtotal;
        
        // Assert
        Assert.Equal(31.50m, subtotal);
    }

    [Fact]
    public void CartItemDto_Subtotal_WithZeroQuantity()
    {
        // Arrange
        var dto = new CartItemDto(1, "Product", 10.00m, 0, "img.jpg");
        
        // Act
        var subtotal = dto.Subtotal;
        
        // Assert
        Assert.Equal(0, subtotal);
    }

    [Fact]
    public void CartItemDto_Properties_SetCorrectly()
    {
        // Arrange & Act
        var dto = new CartItemDto(5, "Test", 99.99m, 2, "test.jpg");
        
        // Assert
        Assert.Equal(5, dto.ProductId);
        Assert.Equal("Test", dto.Name);
        Assert.Equal(99.99m, dto.UnitPrice);
        Assert.Equal(2, dto.Quantity);
        Assert.Equal("test.jpg", dto.ImageUrl);
    }

    [Fact]
    public void PaymentRequest_Properties_SetCorrectly()
    {
        // Arrange & Act
        var req = new PaymentRequest
        {
            CustomerName = "John Doe",
            CustomerEmail = "john@test.com"
        };
        
        // Assert
        Assert.Equal("John Doe", req.CustomerName);
        Assert.Equal("john@test.com", req.CustomerEmail);
    }

    [Fact]
    public void PaymentResult_Success_WithTransactionId()
    {
        // Arrange & Act
        var result = new PaymentResult(true, "TXN123");
        
        // Assert
        Assert.True(result.Success);
        Assert.Equal("TXN123", result.TransactionId);
    }

    [Fact]
    public void PaymentResult_Failure_WithError()
    {
        // Arrange & Act
        var result = new PaymentResult(false, null, "Payment declined");
        
        // Assert
        Assert.False(result.Success);
        Assert.Equal("Payment declined", result.ErrorMessage);
    }

    [Fact]
    public void PaymentResult_Success_NoError()
    {
        // Arrange & Act
        var result = new PaymentResult(true, "TXN456");
        
        // Assert
        Assert.Null(result.ErrorMessage);
    }
}
