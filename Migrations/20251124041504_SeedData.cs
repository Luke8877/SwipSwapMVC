using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwipSwapMVC.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "Name" },
                values: new object[] { 1, "General" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "DateCreated", "Email", "PasswordHash", "PhoneNumber", "Username" },
                values: new object[] { 1, new DateTime(2025, 11, 24, 4, 15, 4, 185, DateTimeKind.Utc).AddTicks(4839), "test@test.com", "fakehash", "0000000000", "TestUser" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "DatePosted", "Description", "ImageUrl", "IsSold", "Name", "Price", "SellerId" },
                values: new object[] { 1, 1, new DateTime(2025, 11, 24, 4, 15, 4, 185, DateTimeKind.Utc).AddTicks(5000), "Seeded test product", null, false, "Test Product", 10.00m, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);
        }
    }
}
