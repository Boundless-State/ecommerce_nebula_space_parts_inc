using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ShippingAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentTransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 30, 11, 2, 25, 896, DateTimeKind.Utc).AddTicks(3869), "Advanced engines and thrusters for your spaceship", "Propulsion Systems" },
                    { 2, new DateTime(2025, 10, 30, 11, 2, 25, 896, DateTimeKind.Utc).AddTicks(4026), "State-of-the-art navigation and control systems", "Navigation & Control" },
                    { 3, new DateTime(2025, 10, 30, 11, 2, 25, 896, DateTimeKind.Utc).AddTicks(4028), "Critical life support systems and equipment", "Life Support" },
                    { 4, new DateTime(2025, 10, 30, 11, 2, 25, 896, DateTimeKind.Utc).AddTicks(4029), "Shield generators and defensive systems", "Weapons & Defense" },
                    { 5, new DateTime(2025, 10, 30, 11, 2, 25, 896, DateTimeKind.Utc).AddTicks(4030), "Reactors and power generation equipment", "Power Systems" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 10, 30, 11, 2, 25, 896, DateTimeKind.Utc).AddTicks(8901), "Revolutionary quantum propulsion system capable of faster-than-light travel. Includes built-in stabilizers and emergency fallback thrusters.", "https://via.placeholder.com/300x300/1a1a2e/0f3460?text=Quantum+Drive", true, "Quantum Flux Drive MK-VII", 2499999.99m, 5 },
                    { 2, 1, new DateTime(2025, 10, 30, 11, 2, 25, 896, DateTimeKind.Utc).AddTicks(8985), "High-efficiency ion thruster system for precise maneuvering in deep space. Features dual-core plasma containment.", "https://via.placeholder.com/300x300/16213e/0f3460?text=Ion+Thruster", true, "Plasma Ion Thruster Array", 875000.00m, 12 },
                    { 3, 2, new DateTime(2025, 10, 30, 11, 2, 25, 896, DateTimeKind.Utc).AddTicks(8987), "AI-powered navigation system with quantum computing core. Calculates optimal routes through asteroid fields and nebulae.", "https://via.placeholder.com/300x300/0f3460/e94560?text=Nav+Matrix", true, "Neural Navigation Matrix", 1250000.00m, 8 },
                    { 4, 2, new DateTime(2025, 10, 30, 11, 2, 25, 896, DateTimeKind.Utc).AddTicks(8989), "Maintains artificial gravity in all ship sections. Essential for long-duration space missions and crew comfort.", "https://via.placeholder.com/300x300/533483/e94560?text=Gravity+Module", true, "Gravity Stabilizer Module", 650000.00m, 15 },
                    { 5, 3, new DateTime(2025, 10, 30, 11, 2, 25, 896, DateTimeKind.Utc).AddTicks(8990), "Advanced life support system that recycles air and water with 99.9% efficiency. Supports up to 50 crew members.", "https://via.placeholder.com/300x300/2d4059/ea5455?text=Air+Processor", true, "Bio-Regenerative Air Processor", 425000.00m, 20 },
                    { 6, 3, new DateTime(2025, 10, 30, 11, 2, 25, 896, DateTimeKind.Utc).AddTicks(8992), "Single-occupant cryogenic sleep chamber for extended interstellar journeys. Includes medical monitoring and auto-revival systems.", "https://via.placeholder.com/300x300/1b262c/f07b3f?text=Cryo+Pod", true, "Cryo-Sleep Chamber Pod", 895000.00m, 7 },
                    { 7, 4, new DateTime(2025, 10, 30, 11, 2, 25, 896, DateTimeKind.Utc).AddTicks(8994), "Military-grade energy shield system. Protects against laser weapons, particle beams, and micro-meteor impacts.", "https://via.placeholder.com/300x300/3d315b/ffc93c?text=Shield+Gen", true, "Photon Shield Generator X-200", 1899999.99m, 3 },
                    { 8, 4, new DateTime(2025, 10, 30, 11, 2, 25, 896, DateTimeKind.Utc).AddTicks(8995), "Long-range detection and tracking system. Identifies threats up to 10 million kilometers away.", "https://via.placeholder.com/300x300/2c003e/f71735?text=Sensor+Suite", true, "Tactical Sensor Suite", 725000.00m, 10 },
                    { 9, 5, new DateTime(2025, 10, 30, 11, 2, 25, 896, DateTimeKind.Utc).AddTicks(8997), "Compact antimatter reactor providing 50 petawatts of continuous power. Includes failsafe containment protocols.", "https://via.placeholder.com/300x300/000000/00d9ff?text=Fusion+Reactor", true, "Antimatter Fusion Reactor", 3250000.00m, 2 },
                    { 10, 5, new DateTime(2025, 10, 30, 11, 2, 25, 896, DateTimeKind.Utc).AddTicks(8998), "Auxiliary power system that harnesses stellar radiation. Perfect for extended missions away from stations.", "https://via.placeholder.com/300x300/120136/035aa6?text=Solar+Sail", true, "Solar Sail Energy Collector", 385000.00m, 25 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
