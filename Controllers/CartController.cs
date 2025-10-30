using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;
using Services.Interfaces;

namespace webshopAI.Controllers;

public class CartController(ICartService cartService, IProductService productService, ILogger<CartController> logger) : Controller
{
    private readonly ICartService _cartService = cartService;
    private readonly IProductService _productService = productService;
    private readonly ILogger<CartController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var items = await _cartService.GetItemsAsync();
        var vm = new CartViewModel
        {
            Items = items.Select(i => new CartItemViewModel
            {
                ProductId = i.ProductId,
                Name = i.Name,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                ImageUrl = i.ImageUrl
            }).ToList()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int id, int qty = 1)
    {
        await _cartService.AddAsync(id, qty);
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, int qty)
    {
        await _cartService.UpdateQuantityAsync(id, qty);
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int id)
    {
        await _cartService.RemoveAsync(id);
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Clear()
    {
        await _cartService.ClearAsync();
        return RedirectToAction("Index");
    }
}
