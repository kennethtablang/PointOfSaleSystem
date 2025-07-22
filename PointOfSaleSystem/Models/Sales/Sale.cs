using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Models.Auth;
using PointOfSaleSystem.Models.Settings;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;

namespace PointOfSaleSystem.Models.Sales
{
    public class Sale
    {
        public int Id { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.Now;

        [Required]
        public string InvoiceNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? ORNumber { get; set; } // For official receipt tracking

        [Required]
        public string CashierId { get; set; }

        [ForeignKey("CashierId")]
        public ApplicationUser Cashier { get; set; }

        public int? CounterId { get; set; }

        [ForeignKey("CounterId")]
        public Counter? Counter { get; set; }

        [Precision(18, 2)]
        public decimal SubTotal { get; set; }

        [Precision(18, 2)]
        public decimal TotalDiscount { get; set; } = 0;

        [Precision(18, 2)]
        public decimal VatAmount { get; set; } //Only VATable items

        [Precision(18, 2)]
        public decimal NonVatAmount { get; set; } //NetAmount

        [Precision(18, 2)]
        public decimal TotalAmount { get; set; }

        public bool IsVoided { get; set; } = false;

        public DateTime? VoidedAt { get; set; }

        public string? VoidedByUserId { get; set; }

        [ForeignKey("VoidedByUserId")]
        public ApplicationUser? VoidedByUser { get; set; }

        [MaxLength(100)]
        public string? Remarks { get; set; }

        public bool IsSeniorCitizen { get; set; } = false;

        public string? SeniorCitizenId { get; set; }

        public ICollection<SaleItem>? SaleItems { get; set; }
        public ICollection<Payment>? Payments { get; set; }
        public ICollection<Discount>? Discounts { get; set; }
        public ICollection<ReturnTransaction>? Returns { get; set; }

        public int? XReadingId { get; set; }
        [ForeignKey("XReadingId")]
        public XReading? XReading { get; set; }

        public int? ZReadingId { get; set; }
        [ForeignKey("ZReadingId")]
        public ZReading? ZReading { get; set; }
    }
}
