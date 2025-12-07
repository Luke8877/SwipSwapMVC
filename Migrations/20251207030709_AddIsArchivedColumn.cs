using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwipSwapMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddIsArchivedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "DatePosted",
                value: new DateTime(2025, 12, 7, 3, 7, 8, 689, DateTimeKind.Utc).AddTicks(5298));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 12, 7, 3, 7, 8, 689, DateTimeKind.Utc).AddTicks(5149));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
