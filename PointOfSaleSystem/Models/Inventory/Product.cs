using PointOfSaleSystem.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PointOfSaleSystem.Models.Inventory
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Barcode { get; set; }

        public bool IsBarcoded { get; set; } = true;

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public int UnitId { get; set; }
        public Unit Unit { get; set; }

        public string? Description { get; set; }

        [Required]
        [Precision(18, 2)]
        public decimal Price { get; set; }

        [Required]
        public TaxType TaxType { get; set; }

        public bool IsPerishable { get; set; } = false;

        public int? ReorderLevel { get; set; }

        public bool IsActive { get; set; } = true;

        public byte[]? ImageData { get; set; }

        [Precision(18, 2)]
        public decimal OnHand { get; set; } = 0m;

        public ICollection<InventoryTransaction>? InventoryTransactions { get; set; } = new List<InventoryTransaction>();
    }
}