using PointOfSaleSystem.Enums;

namespace PointOfSaleSystem.DTOs.Inventory
{
    public class InventoryTransactionReadDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public InventoryActionType ActionType { get; set; }
        public decimal Quantity { get; set; }         // positive for StockIn, negative for StockOut depending on business rules
        public decimal? UnitCost { get; set; }
        public string? ReferenceNumber { get; set; }  // e.g., PO number, sale invoice no.
        public string? Remarks { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? PerformedByUserId { get; set; }
        public string? PerformedByUserName { get; set; }
    }
    public class InventoryTransactionCreateDto
    {
        public int ProductId { get; set; }
        public InventoryActionType ActionType { get; set; }
        public decimal Quantity { get; set; }
        public decimal? UnitCost { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Remarks { get; set; }
        public DateTime? TransactionDate { get; set; }
    }

    // Update DTO (if you plan to support editing transactions)
    public class InventoryTransactionUpdateDto
    {
        public decimal Quantity { get; set; }
        public decimal? UnitCost { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Remarks { get; set; }
        public DateTime? TransactionDate { get; set; }
    }

    // Product stock summary DTO (useful for quick on-hand queries)
    public class ProductStockDto
    {
        public int ProductId { get; set; }
        public decimal OnHand { get; set; }
        public decimal Reserved { get; set; }
    }
}
