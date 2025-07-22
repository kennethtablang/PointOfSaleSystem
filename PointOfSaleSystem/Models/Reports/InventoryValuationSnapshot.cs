using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Models.Reports
{
    public class InventoryValuationSnapshot
    {
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime SnapshotDate { get; set; } // e.g., 2025-07-20

        [Required]
        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalInventoryValue { get; set; } // Total value (Qty × Cost)

        [MaxLength(100)]
        public string? Notes { get; set; } // Optional remarks (e.g., "Month-end count")
    }
}
