using PointOfSaleSystem.Enums;
using PointOfSaleSystem.Models.Inventory;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PointOfSaleSystem.Models.Sales
{
    public class SaleItem
    {
        public int Id { get; set; }

        [Required]
        public int SaleId { get; set; }
        [ForeignKey("SaleId")]
        public Sale Sale { get; set; }

        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Precision(18, 2)]
        public decimal ReturnedQuantity { get; set; } = 0;

        [Required]
        public int UnitId { get; set; }
        [ForeignKey("UnitId")]
        public Unit Unit { get; set; }

        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }

        [Precision(18, 2)]
        public decimal CostPrice { get; set; }

        [Range(0, 100)]
        [Precision(18, 4)]
        public decimal DiscountPercent { get; set; } = 0;

        [Precision(18, 2)]
        public decimal DiscountAmount { get; set; }

        [Precision(18, 2)]
        public decimal ComputedTotal { get; set; }

        public TaxType TaxType { get; set; }

        [Precision(18, 2)]
        public decimal VatAmount { get; set; }

        [Precision(18, 2)]
        public decimal NonVatAmount { get; set; }

        public void CalculateTotal()
        {
            DiscountAmount = Math.Round((UnitPrice * Quantity) * (DiscountPercent / 100), 2);
            ComputedTotal = Math.Round((UnitPrice * Quantity) - DiscountAmount, 2);
        }
    }
}
