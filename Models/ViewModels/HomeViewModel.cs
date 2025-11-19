namespace Models.ViewModels;

/// <summary>
/// ViewModel for the homepage displaying featured products
/// </summary>
public class HomeViewModel
{
    public List<ProductViewModel> FeaturedProducts { get; set; } = new();
    
    public string WelcomeMessage { get; set; } = "Welcome to SpaceShip Parts Store!";
}
