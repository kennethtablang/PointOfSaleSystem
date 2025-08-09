using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PointOfSaleSystem.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductUnitConversions_Products_ProductId",
                table: "ProductUnitConversions");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductUnitConversions_Products_ProductId",
                table: "ProductUnitConversions",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductUnitConversions_Products_ProductId",
                table: "ProductUnitConversions");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductUnitConversions_Products_ProductId",
                table: "ProductUnitConversions",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
