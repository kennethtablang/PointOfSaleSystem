using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Models.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PointOfSaleSystem.Models.Settings
{
    public class XReading
    {
        public int Id { get; set; }

        [Required]
        public DateTime GeneratedAt { get; set; } = DateTime.Now;

        [Required]
        public int CounterId { get; set; }

        public string? CashierId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Precision(18, 4)]
        public decimal TotalSales { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Precision(18, 4)]
        public decimal TotalVAT { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Precision(18, 4)]
        public decimal TotalDiscounts { get; set; }

        [Range(0, double.MaxValue)]
        [Precision(18, 4)]
        public decimal CashInDrawer { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }

        // Navigation
        [ForeignKey("CashierId")]
        public ApplicationUser? Cashier { get; set; }

        [ForeignKey("CounterId")]
        public Counter? Counter { get; set; }

        public string? GeneratedByUserId { get; set; }
        [ForeignKey("GeneratedByUserId")]
        public ApplicationUser? GeneratedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
