using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwipSwapMVC.Migrations
{
    /// <inheritdoc />
    public partial class Migration100000 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "DatePosted",
                value: new DateTime(2025, 11, 26, 0, 30, 39, 118, DateTimeKind.Utc).AddTicks(4267));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 11, 26, 0, 30, 39, 118, DateTimeKind.Utc).AddTicks(4097));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "DatePosted",
                value: new DateTime(2025, 11, 26, 0, 29, 56, 66, DateTimeKind.Utc).AddTicks(3021));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 11, 26, 0, 29, 56, 66, DateTimeKind.Utc).AddTicks(2874));
        }
    }
}
