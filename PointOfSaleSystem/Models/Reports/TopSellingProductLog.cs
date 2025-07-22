using PointOfSaleSystem.Models.Inventory;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Models.Reports
{
    public class TopSellingProductLog
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int QuantitySold { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalRevenue { get; set; }

        [Required]
        public DateTime LoggedDate { get; set; } // The date or week this snapshot was taken

        public string? PeriodLabel { get; set; } // e.g., "Week 29, 2025" or "July 2025"

        // Navigation
        public Product? Product { get; set; }
    }
}
