namespace Models.ViewModels;

/// <summary>
/// ViewModel for displaying product information on the homepage
/// </summary>
public class ProductViewModel
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public decimal Price { get; set; }
    
    public int Stock { get; set; }
    
    public string ImageUrl { get; set; } = string.Empty;
    
    public string CategoryName { get; set; } = string.Empty;
    
    public bool IsInStock => Stock > 0;
}
