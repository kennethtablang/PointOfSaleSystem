using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Inventory
{
    public class StockReceiveItemCreateDto
    {
        [Required]
        public int ProductId { get; set; }
        public int? FromUnitId { get; set; }

        [Required]
        [Range(0.0001, double.MaxValue, ErrorMessage = "QuantityInFromUnit must be > 0")]
        public decimal QuantityInFromUnit { get; set; }

        [Range(0.0001, double.MaxValue, ErrorMessage = "ConvertedQuantity must be > 0")]
        public decimal? ConvertedQuantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? UnitCost { get; set; }

        [MaxLength(250)]
        public string? Remarks { get; set; }

        [MaxLength(100)]
        public string? BatchNumber { get; set; }

        public DateTime? ExpiryDate { get; set; }
    }

    public class StockReceiveItemReadDto
    {
        public int Id { get; set; }
        public int StockReceiveId { get; set; }
        public int PurchaseOrderId { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;

        public int? FromUnitId { get; set; }
        public string? FromUnitName { get; set; }

        public decimal QuantityInFromUnit { get; set; }

        public decimal Quantity { get; set; }

        public decimal? UnitCost { get; set; }

        public string? BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public string? Remarks { get; set; }

        public int? InventoryTransactionId { get; set; }
        public DateTime ReceivedDate { get; set; }

        public string ReceivedByUserId { get; set; } = string.Empty;
        public string? ReceivedByUserName { get; set; }
    }

    public class StockReceiveItemUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Range(0.0001, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? UnitCost { get; set; }

        [MaxLength(250)]
        public string? Remarks { get; set; }

        public string? BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
