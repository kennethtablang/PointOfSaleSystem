using PointOfSaleSystem.Models.Auth;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Models.Reports
{
    public class CashierSalesSummary
    {
        public int Id { get; set; }

        [Required]
        public string CashierId { get; set; } = string.Empty;

        [ForeignKey("CashierId")]
        public ApplicationUser? Cashier { get; set; }

        [Required]
        public int CounterId { get; set; }

        [ForeignKey("CounterId")]
        public Settings.Counter? Counter { get; set; }

        [Required]
        public DateTime ShiftStart { get; set; }

        [Required]
        public DateTime ShiftEnd { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalSales { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalDiscounts { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalVAT { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalNonVAT { get; set; }

        public int NumberOfTransactions { get; set; }

        [MaxLength(100)]
        public string? Notes { get; set; }
    }
}
