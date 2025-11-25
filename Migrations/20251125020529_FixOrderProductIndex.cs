using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwipSwapMVC.Migrations
{
    /// <inheritdoc />
    public partial class FixOrderProductIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_ProductId",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "DatePosted",
                value: new DateTime(2025, 11, 25, 2, 5, 28, 664, DateTimeKind.Utc).AddTicks(757));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 11, 25, 2, 5, 28, 664, DateTimeKind.Utc).AddTicks(621));

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProductId",
                table: "Orders",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_ProductId",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "DatePosted",
                value: new DateTime(2025, 11, 24, 4, 15, 4, 185, DateTimeKind.Utc).AddTicks(5000));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 11, 24, 4, 15, 4, 185, DateTimeKind.Utc).AddTicks(4839));

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProductId",
                table: "Orders",
                column: "ProductId",
                unique: true);
        }
    }
}
