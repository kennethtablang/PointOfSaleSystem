using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Sales
{
    public class SaleItemCreateDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int UnitId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        // CostPrice snapshot for historical/cogs
        public decimal? CostPrice { get; set; }

        [Range(0, 100)]
        public decimal DiscountPercent { get; set; } = 0m;

        public string? Remarks { get; set; }
    }

    public class SaleItemReadDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int UnitId { get; set; }
        public string? UnitName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal CostPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ComputedTotal { get; set; }
        public decimal ReturnedQuantity { get; set; }
    }
}
