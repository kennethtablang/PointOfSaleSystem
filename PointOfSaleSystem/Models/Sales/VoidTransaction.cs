using PointOfSaleSystem.Models.Auth;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Models.Sales
{
    public class VoidTransaction
    {
        public int Id { get; set; }

        [Required]
        public int SaleId { get; set; }

        [ForeignKey("SaleId")]
        public Sale Sale { get; set; }

        [Required]
        public string VoidedByUserId { get; set; }

        [ForeignKey("VoidedByUserId")]
        public ApplicationUser VoidedBy { get; set; }

        [Required]
        public DateTime VoidedAt { get; set; } = DateTime.Now;

        [MaxLength(250)]
        public string Reason { get; set; } = string.Empty;

        // Optional: IP or machine name of the terminal
        [MaxLength(100)]
        public string? TerminalIdentifier { get; set; }

        // Optional: Track original cashier or user who made the sale
        public string? OriginalCashierUserId { get; set; }

        [ForeignKey("OriginalCashierUserId")]
        public ApplicationUser? OriginalCashier { get; set; }
    }
}
