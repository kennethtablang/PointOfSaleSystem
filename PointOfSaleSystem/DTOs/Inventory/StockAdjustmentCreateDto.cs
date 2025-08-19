using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Inventory
{
    public class StockAdjustmentCreateDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(typeof(decimal), "-999999.00", "999999.00", ErrorMessage = "Quantity must be non-zero and within bounds.")]
        public decimal Quantity { get; set; }

        public int? UnitId { get; set; }

        [MaxLength(150)]
        public string? Reason { get; set; }

        // User performing adjustment (set in controller/service)
        public string AdjustedByUserId { get; set; } = string.Empty;

        // Usually false unless system auto-generates it
        public bool IsSystemGenerated { get; set; } = false;
    }
    public class StockAdjustmentUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Range(typeof(decimal), "-999999.00", "999999.00", ErrorMessage = "Quantity must be non-zero and within bounds.")]
        public decimal Quantity { get; set; }

        public int? UnitId { get; set; }

        [MaxLength(150)]
        public string? Reason { get; set; }

        public bool IsSystemGenerated { get; set; } = false;
    }

    public class StockAdjustmentReadDto
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public string? ProductName { get; set; }

        public decimal Quantity { get; set; }

        public int? UnitId { get; set; }
        public string? UnitName { get; set; }

        public string? Reason { get; set; }

        public DateTime AdjustmentDate { get; set; }

        public string AdjustedByUserId { get; set; } = string.Empty;
        public string? AdjustedByUserName { get; set; }

        public bool IsSystemGenerated { get; set; }

        public int? InventoryTransactionId { get; set; }
    }

    public class StockAdjustmentListDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string? UnitName { get; set; }
        public string? Reason { get; set; }
        public DateTime AdjustmentDate { get; set; }
        public string AdjustedByUserName { get; set; } = string.Empty;
    }
}
