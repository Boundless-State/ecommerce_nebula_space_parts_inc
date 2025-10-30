using Data;
using Microsoft.EntityFrameworkCore;
using Services.Implementations;
using Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add session for Cart
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Database: SQL Server LocalDB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\MSSQLLocalDB;Database=webshopAI;Trusted_Connection=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<WebshopDbContext>(options =>
    options.UseSqlServer(connectionString));
    
// Register application services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();

// Payment provider
var provider = builder.Configuration.GetValue<string>("PaymentProvider") ?? "mock";
if (provider.Equals("stripe", StringComparison.OrdinalIgnoreCase))
{
    // Placeholder: when adding Stripe later, swap implementation
    builder.Services.AddScoped<IPaymentService, MockPaymentProvider>();
}
else
{
    builder.Services.AddScoped<IPaymentService, MockPaymentProvider>();
}

var app = builder.Build();

// Apply migrations / create database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WebshopDbContext>();
    await db.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Expose Program for WebApplicationFactory in tests
public partial class Program { }
