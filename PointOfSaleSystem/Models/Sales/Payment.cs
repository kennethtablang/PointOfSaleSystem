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

        public string? ReferenceNumber { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public PaymentStatus Status { get; set; } = PaymentStatus.Completed;

        [MaxLength(50)]
        public string? Terminal { get; set; }

        // For traceability of who processed the payment
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        // New: amount of change given back to customer for cash transactions
        [Precision(18, 2)]
        public decimal? ChangeAmount { get; set; }
    }
}
