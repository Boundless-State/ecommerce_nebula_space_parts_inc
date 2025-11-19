namespace Models.Entities;

/// <summary>
/// Represents a product category in the webshop
/// </summary>
public class Category
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
