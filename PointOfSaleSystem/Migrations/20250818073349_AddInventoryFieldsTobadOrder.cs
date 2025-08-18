using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PointOfSaleSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryFieldsTobadOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InventoryTransactionId",
                table: "BadOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSystemGenerated",
                table: "BadOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_BadOrders_InventoryTransactionId",
                table: "BadOrders",
                column: "InventoryTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_BadOrders_InventoryTransactions_InventoryTransactionId",
                table: "BadOrders",
                column: "InventoryTransactionId",
                principalTable: "InventoryTransactions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BadOrders_InventoryTransactions_InventoryTransactionId",
                table: "BadOrders");

            migrationBuilder.DropIndex(
                name: "IX_BadOrders_InventoryTransactionId",
                table: "BadOrders");

            migrationBuilder.DropColumn(
                name: "InventoryTransactionId",
                table: "BadOrders");

            migrationBuilder.DropColumn(
                name: "IsSystemGenerated",
                table: "BadOrders");
        }
    }
}
