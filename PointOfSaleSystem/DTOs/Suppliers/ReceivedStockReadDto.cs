namespace PointOfSaleSystem.DTOs.Suppliers
{
    public class ReceivedStockReadDto
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int QuantityReceived { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }
        public string? ReceivedByUserName { get; set; }
    }

    // Create DTO
    public class ReceivedStockCreateDto
    {
        public int PurchaseOrderId { get; set; }
        public int ProductId { get; set; }
        public int QuantityReceived { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }
    }
}
