using PointOfSaleSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Inventory
{
    public class ProductCreateDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Barcode { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int UnitId { get; set; }

        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public TaxType VatType { get; set; }

        public bool IsPerishable { get; set; } = false;

        public int? ReorderLevel { get; set; }
    }

    public class ProductUpdateDto : ProductCreateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Barcode { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int UnitId { get; set; }

        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public TaxType VatType { get; set; }

        public bool IsPerishable { get; set; } = false;

        public int? ReorderLevel { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class ProductViewDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Barcode { get; set; }

        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public int UnitId { get; set; }
        public string UnitName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public TaxType VatType { get; set; }

        public bool IsPerishable { get; set; }

        public int? ReorderLevel { get; set; }

        public bool IsActive { get; set; }
    }
}
