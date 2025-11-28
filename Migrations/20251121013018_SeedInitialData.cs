using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SwipSwapMVC.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "Name" },
                values: new object[,]
                {
                    { 1, "Electronics" },
                    { 2, "Vehicles" },
                    { 3, "Sports" },
                    { 4, "Home & Garden" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "DateCreated", "Email", "PasswordHash", "PhoneNumber", "Username" },
                values: new object[] { 1, new DateTime(2025, 11, 20, 18, 30, 17, 985, DateTimeKind.Local).AddTicks(6725), "demo@example.com", "Password123!", null, "Demo User" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "DatePosted", "Description", "ImageUrl", "IsSold", "Name", "Price", "SellerId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 11, 20, 18, 30, 17, 985, DateTimeKind.Local).AddTicks(6895), "256GB — Deep Purple — excellent condition", "/uploads/iphone14.jpg", false, "iPhone 14 Pro", 1199.99m, 1 },
                    { 2, 1, new DateTime(2025, 11, 20, 18, 30, 17, 985, DateTimeKind.Local).AddTicks(6899), "RTX 3070 — 16GB RAM — 1TB SSD", "/uploads/laptop.jpg", false, "Gaming Laptop", 1599.00m, 1 },
                    { 3, 3, new DateTime(2025, 11, 20, 18, 30, 17, 985, DateTimeKind.Local).AddTicks(6903), "Aluminum frame — good condition", "/uploads/bike.jpg", false, "Mountain Bike", 450.00m, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);
        }
    }
}
