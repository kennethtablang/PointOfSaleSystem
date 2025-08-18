using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Enums;
using PointOfSaleSystem.Models.Auth;

namespace PointOfSaleSystem.Models.Inventory
{
    public class InventoryTransaction
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        [Required]
        public InventoryActionType ActionType { get; set; } // StockIn, Sale, Return, etc.

        [Required]
        [Precision(18, 2)]
        public decimal Quantity { get; set; } // Can be positive or negative depending on action

        [Precision(18, 2)]
        public decimal? UnitCost { get; set; } // Used for StockIn or adjustment

        public string? ReferenceNumber { get; set; } // Link to PO, Sale, Return, etc.

        public string? Remarks { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        public string? PerformedById { get; set; }

        [ForeignKey("PerformedById")]
        public ApplicationUser? PerformedByUser { get; set; }

        public int? ReferenceId { get; set; }
        public string? ReferenceType { get; set; }
    }
}
