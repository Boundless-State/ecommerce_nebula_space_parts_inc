using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Data;

/// <summary>
/// Main database context for the webshop
/// </summary>
public class WebshopDbContext : DbContext
{
    public WebshopDbContext(DbContextOptions<WebshopDbContext> options) : base(options)
    {
    }
    
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Description).IsRequired().HasMaxLength(2000);
            entity.Property(p => p.Price).HasPrecision(18, 2);
            entity.Property(p => p.ImageUrl).HasMaxLength(500);
            
            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Description).HasMaxLength(500);
        });
        
        // Configure Order entity
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.OrderNumber).IsRequired().HasMaxLength(50);
            entity.Property(o => o.CustomerName).IsRequired().HasMaxLength(200);
            entity.Property(o => o.CustomerEmail).IsRequired().HasMaxLength(200);
            entity.Property(o => o.ShippingAddress).IsRequired().HasMaxLength(500);
            entity.Property(o => o.TotalAmount).HasPrecision(18, 2);
            entity.Property(o => o.Status).HasMaxLength(50);
        });
        
        // Configure OrderItem entity
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(oi => oi.Id);
            entity.Property(oi => oi.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(oi => oi.UnitPrice).HasPrecision(18, 2);
            entity.Property(oi => oi.TotalPrice).HasPrecision(18, 2);
            
            entity.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Seed data
        SeedData(modelBuilder);
    }
    
    private void SeedData(ModelBuilder modelBuilder)
    {
        // Use a fixed timestamp for seed data to avoid PendingModelChangesWarning
        var now = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc);

        // Seed Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Propulsion Systems", Description = "Advanced engines and thrusters for your spaceship", CreatedAt = now },
            new Category { Id = 2, Name = "Navigation & Control", Description = "State-of-the-art navigation and control systems", CreatedAt = now },
            new Category { Id = 3, Name = "Life Support", Description = "Critical life support systems and equipment", CreatedAt = now },
            new Category { Id = 4, Name = "Weapons & Defense", Description = "Shield generators and defensive systems", CreatedAt = now },
            new Category { Id = 5, Name = "Power Systems", Description = "Reactors and power generation equipment", CreatedAt = now }
        );
        
        
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Quantum Flux Drive MK-VII",
                Description = "Revolutionary quantum propulsion system capable of faster-than-light travel. Includes built-in stabilizers and emergency fallback thrusters.",
                Price = 2499999.99m,
                Stock = 5,
                ImageUrl = "quantum_flux_drive_mkvii.png",
                CategoryId = 1,
                IsActive = true,
                CreatedAt = now
            },
            new Product
            {
                Id = 2,
                Name = "Plasma Ion Thruster Array",
                Description = "High-efficiency ion thruster system for precise maneuvering in deep space. Features dual-core plasma containment.",
                Price = 875000.00m,
                Stock = 12,
                ImageUrl = "plasma_ion_thruster_array.png",
                CategoryId = 1,
                IsActive = true,
                CreatedAt = now
            },
            new Product
            {
                Id = 3,
                Name = "Neural Navigation Matrix",
                Description = "AI-powered navigation system with quantum computing core. Calculates optimal routes through asteroid fields and nebulae.",
                Price = 1250000.00m,
                Stock = 8,
                ImageUrl = "neural_navigation_matrix.png",
                CategoryId = 2,
                IsActive = true,
                CreatedAt = now
            },
            new Product
            {
                Id = 4,
                Name = "Gravity Stabilizer Module",
                Description = "Maintains artificial gravity in all ship sections. Essential for long-duration space missions and crew comfort.",
                Price = 650000.00m,
                Stock = 15,
                ImageUrl = "gravity_stabilizer_module.png",
                CategoryId = 2,
                IsActive = true,
                CreatedAt = now
            },
            new Product
            {
                Id = 5,
                Name = "Bio-Regenerative Air Processor",
                Description = "Advanced life support system that recycles air and water with 99.9% efficiency. Supports up to 50 crew members.",
                Price = 425000.00m,
                Stock = 20,
                ImageUrl = "bio-regenerative_air_processor.png",
                CategoryId = 3,
                IsActive = true,
                CreatedAt = now
            },
            new Product
            {
                Id = 6,
                Name = "Cryo-Sleep Chamber Pod",
                Description = "Single-occupant cryogenic sleep chamber for extended interstellar journeys. Includes medical monitoring and auto-revival systems.",
                Price = 895000.00m,
                Stock = 7,
                ImageUrl = "cryo-sleep_chamber_pro.png",
                CategoryId = 3,
                IsActive = true,
                CreatedAt = now
            },
            new Product
            {
                Id = 7,
                Name = "Photon Shield Generator X-200",
                Description = "Military-grade energy shield system. Protects against laser weapons, particle beams, and micro-meteor impacts.",
                Price = 1899999.99m,
                Stock = 3,
                ImageUrl = "photon_shield_generator_x200.png",
                CategoryId = 4,
                IsActive = true,
                CreatedAt = now
            },
            new Product
            {
                Id = 8,
                Name = "Tactical Sensor Suite",
                Description = "Long-range detection and tracking system. Identifies threats up to 10 million kilometers away.",
                Price = 725000.00m,
                Stock = 10,
                ImageUrl = "tactical_sensor_suite.png",
                CategoryId = 4,
                IsActive = true,
                CreatedAt = now
            },
            new Product
            {
                Id = 9,
                Name = "Antimatter Fusion Reactor",
                Description = "Compact antimatter reactor providing 50 petawatts of continuous power. Includes failsafe containment protocols.",
                Price = 3250000.00m,
                Stock = 2,
                ImageUrl = "antimatter_fusion_reactor.png",
                CategoryId = 5,
                IsActive = true,
                CreatedAt = now
            },
            new Product
            {
                Id = 10,
                Name = "Solar Sail Energy Collector",
                Description = "Auxiliary power system that harnesses stellar radiation. Perfect for extended missions away from stations.",
                Price = 385000.00m,
                Stock = 25,
                ImageUrl = "solar_sail_energy_collector.png",
                CategoryId = 5,
                IsActive = true,
                CreatedAt = now
            }
        );
    }
}
