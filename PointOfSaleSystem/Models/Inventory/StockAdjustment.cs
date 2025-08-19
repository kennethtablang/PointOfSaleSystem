using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PointOfSaleSystem.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace PointOfSaleSystem.Models.Inventory
{
    public class StockAdjustment
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        [Required]
        [Precision(18,2)]
        public decimal Quantity { get; set; } // + or - depending on adjustment

        public int? UnitId { get; set; }
        [ForeignKey("UnitId")]
        public Unit? Unit { get; set; }

        [MaxLength(150)]
        public string? Reason { get; set; }

        [Required]
        public DateTime AdjustmentDate { get; set; } = DateTime.Now;

        [Required]
        public string AdjustedByUserId { get; set; } = string.Empty;

        [ForeignKey("AdjustedByUserId")]
        public ApplicationUser? AdjustedByUser { get; set; }

        public bool IsSystemGenerated { get; set; } = false;

        public int? InventoryTransactionId { get; set; }

        [ForeignKey("InventoryTransactionId")]
        public InventoryTransaction? InventoryTransaction { get; set; }
    }
}
