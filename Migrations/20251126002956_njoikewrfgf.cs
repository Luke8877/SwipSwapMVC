using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwipSwapMVC.Migrations
{
    /// <inheritdoc />
    public partial class njoikewrfgf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "DatePosted",
                value: new DateTime(2025, 11, 25, 22, 13, 51, 756, DateTimeKind.Utc).AddTicks(637));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 11, 25, 22, 13, 51, 756, DateTimeKind.Utc).AddTicks(506));
        }
    }
}
