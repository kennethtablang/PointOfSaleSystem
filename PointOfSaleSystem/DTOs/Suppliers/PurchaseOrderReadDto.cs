using PointOfSaleSystem.DTOs.Inventory;
using PointOfSaleSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Suppliers
{
    public class PurchaseOrderCreateDto
    {
        [Required]
        public int SupplierId { get; set; }

        [MaxLength(200)]
        public string PurchaseOrderNumber { get; set; } = string.Empty;

        public DateTime? OrderDate { get; set; } // optional; server will set UtcNow if not provided

        public DateTime? ExpectedDeliveryDate { get; set; }

        public string? Remarks { get; set; }

        public List<PurchaseOrderItemCreateDto> Items { get; set; } = new();
    }

    public class PurchaseOrderReadDto
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public string? SupplierName { get; set; }

        public string PurchaseOrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string? Remarks { get; set; }
        public decimal TotalCost { get; set; }

        public List<PurchaseOrderItemReadDto> Items { get; set; } = new();
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<ReceivedStockReadDto>? ReceivedStocks { get; set; } = new();

        public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
        public string? CreatedByUserName { get; internal set; }
    }

    public class PurchaseOrderUpdateDto
    {
        [Required] public int Id { get; set; }

        [Required] public int SupplierId { get; set; }

        [Required]
        [MaxLength(200)]
        public string PurchaseOrderNumber { get; set; } = string.Empty;

        public DateTime? ExpectedDeliveryDate { get; set; }
        public string? Remarks { get; set; }

        [Required]
        public List<PurchaseOrderItemUpdateDto> Items { get; set; } = new();
    }
}
