using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PointOfSaleSystem.Migrations
{
    /// <inheritdoc />
    public partial class StockReceiveItemsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockReceiveItem_InventoryTransactions_InventoryTransactionId",
                table: "StockReceiveItem");

            migrationBuilder.DropForeignKey(
                name: "FK_StockReceiveItem_Products_ProductId",
                table: "StockReceiveItem");

            migrationBuilder.DropForeignKey(
                name: "FK_StockReceiveItem_StockReceives_StockReceiveId",
                table: "StockReceiveItem");

            migrationBuilder.DropForeignKey(
                name: "FK_StockReceiveItem_Units_FromUnitId",
                table: "StockReceiveItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StockReceiveItem",
                table: "StockReceiveItem");

            migrationBuilder.RenameTable(
                name: "StockReceiveItem",
                newName: "StockReceiveItems");

            migrationBuilder.RenameIndex(
                name: "IX_StockReceiveItem_StockReceiveId",
                table: "StockReceiveItems",
                newName: "IX_StockReceiveItems_StockReceiveId");

            migrationBuilder.RenameIndex(
                name: "IX_StockReceiveItem_ProductId",
                table: "StockReceiveItems",
                newName: "IX_StockReceiveItems_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_StockReceiveItem_InventoryTransactionId",
                table: "StockReceiveItems",
                newName: "IX_StockReceiveItems_InventoryTransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_StockReceiveItem_FromUnitId",
                table: "StockReceiveItems",
                newName: "IX_StockReceiveItems_FromUnitId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockReceiveItems",
                table: "StockReceiveItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockReceiveItems_InventoryTransactions_InventoryTransactionId",
                table: "StockReceiveItems",
                column: "InventoryTransactionId",
                principalTable: "InventoryTransactions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockReceiveItems_Products_ProductId",
                table: "StockReceiveItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockReceiveItems_StockReceives_StockReceiveId",
                table: "StockReceiveItems",
                column: "StockReceiveId",
                principalTable: "StockReceives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockReceiveItems_Units_FromUnitId",
                table: "StockReceiveItems",
                column: "FromUnitId",
                principalTable: "Units",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockReceiveItems_InventoryTransactions_InventoryTransactionId",
                table: "StockReceiveItems");

            migrationBuilder.DropForeignKey(
                name: "FK_StockReceiveItems_Products_ProductId",
                table: "StockReceiveItems");

            migrationBuilder.DropForeignKey(
                name: "FK_StockReceiveItems_StockReceives_StockReceiveId",
                table: "StockReceiveItems");

            migrationBuilder.DropForeignKey(
                name: "FK_StockReceiveItems_Units_FromUnitId",
                table: "StockReceiveItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StockReceiveItems",
                table: "StockReceiveItems");

            migrationBuilder.RenameTable(
                name: "StockReceiveItems",
                newName: "StockReceiveItem");

            migrationBuilder.RenameIndex(
                name: "IX_StockReceiveItems_StockReceiveId",
                table: "StockReceiveItem",
                newName: "IX_StockReceiveItem_StockReceiveId");

            migrationBuilder.RenameIndex(
                name: "IX_StockReceiveItems_ProductId",
                table: "StockReceiveItem",
                newName: "IX_StockReceiveItem_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_StockReceiveItems_InventoryTransactionId",
                table: "StockReceiveItem",
                newName: "IX_StockReceiveItem_InventoryTransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_StockReceiveItems_FromUnitId",
                table: "StockReceiveItem",
                newName: "IX_StockReceiveItem_FromUnitId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockReceiveItem",
                table: "StockReceiveItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockReceiveItem_InventoryTransactions_InventoryTransactionId",
                table: "StockReceiveItem",
                column: "InventoryTransactionId",
                principalTable: "InventoryTransactions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockReceiveItem_Products_ProductId",
                table: "StockReceiveItem",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockReceiveItem_StockReceives_StockReceiveId",
                table: "StockReceiveItem",
                column: "StockReceiveId",
                principalTable: "StockReceives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockReceiveItem_Units_FromUnitId",
                table: "StockReceiveItem",
                column: "FromUnitId",
                principalTable: "Units",
                principalColumn: "Id");
        }
    }
}
