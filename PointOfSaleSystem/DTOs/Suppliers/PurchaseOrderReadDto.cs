using PointOfSaleSystem.Enums;

namespace PointOfSaleSystem.DTOs.Suppliers
{
    public class PurchaseOrderReadDto
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string PurchaseOrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public bool IsReceived { get; set; }
        public string? Remarks { get; set; }
        public PurchaseOrderStatus Status { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public decimal TotalCost { get; set; }
        public List<PurchaseItemReadDto> PurchaseItems { get; set; } = new();
        public List<ReceivedStockReadDto> ReceivedStocks { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public string? CreatedByUserName { get; set; }
    }
    // Create DTO - received from frontend
    public class PurchaseOrderCreateDto
    {
        public int SupplierId { get; set; }
        public string? Remarks { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public List<PurchaseItemCreateDto> PurchaseItems { get; set; } = new();
    }

    // Update DTO - received from frontend for editing
    public class PurchaseOrderUpdateDto
    {
        public int SupplierId { get; set; }
        public string? Remarks { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public PurchaseOrderStatus Status { get; set; }
    }
}
