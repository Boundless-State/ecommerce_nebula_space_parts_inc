using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace webshopAI.ViewComponents;

public class CartIconViewComponent(ICartService cartService) : ViewComponent
{
    private readonly ICartService _cartService = cartService;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var count = await _cartService.GetItemCountAsync();
        return View(count);
    }
}
