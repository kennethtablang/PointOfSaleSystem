namespace PointOfSaleSystem.DTOs.Suppliers
{
    //Read DTO
    public class PurchaseItemReadDto
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal CostPerUnit { get; set; }
        public int Quantity { get; set; }
        public int ReceivedQuantity { get; set; }
        public string? Notes { get; set; }
        public decimal TotalCost => Quantity * CostPerUnit;
    }

    // Create DTO
    public class PurchaseItemCreateDto
    {
        public int ProductId { get; set; }
        public decimal CostPerUnit { get; set; }
        public int Quantity { get; set; }
        public string? Notes { get; set; }
    }

    // Update DTO
    public class PurchaseItemUpdateDto
    {
        public decimal CostPerUnit { get; set; }
        public int Quantity { get; set; }
        public string? Notes { get; set; }
    }
}
