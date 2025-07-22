using PointOfSaleSystem.Models.Inventory;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PointOfSaleSystem.Models.Reports
{
    public class ProductSalesHistory
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        [Required]
        public int QuantitySold { get; set; }

        [Required]
        [Precision(18, 2)]
        public decimal TotalSalesAmount { get; set; }

        [Precision(18, 2)]
        public decimal TotalDiscountAmount { get; set; } = 0;

        [Precision(18, 2)]
        public decimal TotalVAT { get; set; } = 0;

        public string? Note { get; set; } // Optional note (e.g., promo day, event, etc.)

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
