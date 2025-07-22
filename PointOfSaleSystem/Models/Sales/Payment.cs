using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PointOfSaleSystem.Enums;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Models.Auth;

namespace PointOfSaleSystem.Models.Sales
{
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        public int SaleId { get; set; }

        [ForeignKey("SaleId")]
        public Sale Sale { get; set; }

        [Required]
        public PaymentMethod Method { get; set; }

        [Precision(18, 2)]
        public decimal Amount { get; set; }

        // fields for reference numbers, e.g., GCash or card transaction IDs
        public string? ReferenceNumber { get; set; }

        // timestamp and traceability
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public PaymentStatus Status { get; set; } = PaymentStatus.Completed;

        // For example, Cash - Drawer 1, GCash - Device X
        [MaxLength(50)]
        public string? Terminal { get; set; }

        public string? UserId { get; set; } // NEW
        public ApplicationUser? User { get; set; } // Navigation property
    }
}
