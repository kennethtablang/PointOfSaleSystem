using PointOfSaleSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Inventory
{
    public class ProductCreateDto
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Barcode { get; set; }

        public bool IsBarcoded { get; set; } = true;

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int UnitId { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public TaxType TaxType { get; set; }

        public bool IsPerishable { get; set; } = false;

        public int? ReorderLevel { get; set; }

        // Base64-encoded image data (optional)
        public string? ImageBase64 { get; set; }
    }

    public class ProductUpdateDto
    {
        [Required]
        public int Id { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Barcode { get; set; }

        public bool IsBarcoded { get; set; } = true;

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int UnitId { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public TaxType TaxType { get; set; }

        public bool IsPerishable { get; set; } = false;

        public int? ReorderLevel { get; set; }

        // Base64-encoded image data (optional)
        public string? ImageBase64 { get; set; }
    }

    public class ProductViewDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Barcode { get; set; }

        public bool IsBarcoded { get; set; }

        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public int UnitId { get; set; }
        public string? UnitName { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public TaxType TaxType { get; set; }

        public bool IsPerishable { get; set; }

        public int? ReorderLevel { get; set; }

        public bool IsActive { get; set; }

        // Base64-encoded image data for display (optional)
        public string? ImageBase64 { get; set; }
    }
}
