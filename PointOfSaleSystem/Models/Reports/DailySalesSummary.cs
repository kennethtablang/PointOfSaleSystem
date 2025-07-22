using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PointOfSaleSystem.Models.Reports
{
    public class DailySalesSummary
    {
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        [Required]
        [Precision(18, 4)]
        public decimal TotalSales { get; set; }

        [Precision(18, 4)]
        public decimal TotalReturns { get; set; } = 0;

        [Precision(18, 4)]
        public decimal TotalDiscounts { get; set; } = 0;

        [Precision(18, 4)]
        public decimal TotalVAT { get; set; } = 0;

        [Precision(18, 4)]
        public decimal TotalNonVAT { get; set; } = 0;

        [Precision(18, 4)]
        public decimal TotalVATExempt { get; set; } = 0;

        [Precision(18, 4)]
        public decimal TotalZeroRated { get; set; } = 0;

        public int NumberOfTransactions { get; set; } = 0;

        public int NumberOfVoids { get; set; } = 0;

        public bool IsZReadCompleted { get; set; } = false;

        public DateTime? GeneratedAt { get; set; } // timestamp when this summary was created

        [MaxLength(100)]
        public string? GeneratedBy { get; set; } // username or ID of who triggered the generation
    }
}
