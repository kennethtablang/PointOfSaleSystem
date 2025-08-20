using PointOfSaleSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Sales
{
    public class SaleCreateDto
    {
        // Cashier is typically set server-side from authenticated user.
        public DateTime? SaleDate { get; set; }

        // Allow client to pass invoice number OR server can generate.
        public string? InvoiceNumber { get; set; }

        public int? CounterId { get; set; }

        [Required]
        [MinLength(1)]
        public List<SaleItemCreateDto> Items { get; set; } = new();

        // Optionally record payments while creating sale
        public List<PaymentCreateDto>? Payments { get; set; }

        public bool IsSeniorCitizen { get; set; } = false;

        public string? Remarks { get; set; }
    }

    public class SaleReadDto
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string CashierId { get; set; } = string.Empty;
        public string? CashierName { get; set; }

        public decimal SubTotal { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal NonVatAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public SaleStatus Status { get; set; }
        public bool IsFullyRefunded { get; set; }
        public DateTime? RefundedAt { get; set; }

        public List<SaleItemReadDto> Items { get; set; } = new();
        public List<PaymentReadDto> Payments { get; set; } = new();
        public List<DiscountReadDto> Discounts { get; set; } = new();
    }
}
