using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Models.Inventory
{
    public class StockReceiveItem
    {
        public int Id { get; set; }

        [Required]
        public int StockReceiveId { get; set; }
        public StockReceive? StockReceive { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        // The unit the receiver used (e.g., Box)
        public int? FromUnitId { get; set; }
        public Unit? FromUnit { get; set; }

        // Quantity entered in the FromUnit
        [Required]
        [Precision(18, 2)]
        public decimal QuantityInFromUnit { get; set; }

        // Converted / canonical stored quantity (e.g., pieces) — maps to inventory Qty
        [Required]
        [Precision(18, 2)]
        public decimal Quantity { get; set; }

        [Precision(18, 2)]
        public decimal? UnitCost { get; set; }

        [MaxLength(100)]
        public string? BatchNumber { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [MaxLength(250)]
        public string? Remarks { get; set; }

        // Optional link to a created InventoryTransaction for traceability
        public int? InventoryTransactionId { get; set; }
        public InventoryTransaction? InventoryTransaction { get; set; }
    }
}
