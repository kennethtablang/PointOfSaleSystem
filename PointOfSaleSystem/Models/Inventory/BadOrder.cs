using PointOfSaleSystem.Models.Auth;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Models.Inventory
{
    public class BadOrder
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        [Required]
        [Range(0, 999999, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; } // Always a positive number

        [Required]
        [MaxLength(100)]
        public string Reason { get; set; } = string.Empty; // e.g., "Expired", "Damaged", "Contaminated"

        [MaxLength(250)]
        public string? Remarks { get; set; } // Optional note like batch no, expiry date, etc.

        [Required]
        public DateTime BadOrderDate { get; set; } = DateTime.Now;

        [Required]
        public string ReportedByUserId { get; set; } = string.Empty;

        [ForeignKey("ReportedByUserId")]
        public ApplicationUser? ReportedByUser { get; set; }
    }
}
