using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Services.Interfaces;

namespace webshopAI.Controllers;

public class ProductsController(IProductService productService, ILogger<ProductsController> logger) : Controller
{
    private readonly IProductService _productService = productService;
    private readonly ILogger<ProductsController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Index(string? q, int? categoryId)
    {
        var products = await _productService.SearchAsync(q, categoryId);
        ViewBag.Categories = await _productService.GetCategoriesAsync();
        ViewBag.Query = q;
        ViewBag.CategoryId = categoryId;
        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null) return NotFound();
        return View(product);
    }
}
