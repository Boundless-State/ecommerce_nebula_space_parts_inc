namespace Models.Entities;

/// <summary>
/// Represents a customer order
/// </summary>
public class Order
{
    public int Id { get; set; }
    
    public string OrderNumber { get; set; } = string.Empty;
    
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    
    public decimal TotalAmount { get; set; }
    
    public string CustomerName { get; set; } = string.Empty;
    
    public string CustomerEmail { get; set; } = string.Empty;
    
    public string ShippingAddress { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Pending"; // Pending, Confirmed, Shipped, Delivered, Cancelled
    
    public string? PaymentTransactionId { get; set; }
    
    public DateTime? PaymentDate { get; set; }
    
    // Navigation property
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
