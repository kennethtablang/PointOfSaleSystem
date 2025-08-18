using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Suppliers
{
    public class ReceiveStockCreateDto
    {
        [Required]
        public int PurchaseOrderId { get; set; }

        [Required]
        public int PurchaseOrderItemId { get; set; }

        [Required]
        [Range(0.0001, double.MaxValue)]
        public decimal QuantityReceived { get; set; }

        public DateTime? ReceivedDate { get; set; }

        [MaxLength(200)]
        public string? ReferenceNumber { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    public class ReceivedStockReadDto
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public int PurchaseOrderItemId { get; set; }
        public int ProductId { get; set; }               // convenience
        public string? ProductName { get; set; }
        public decimal QuantityReceived { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }
        public string? ReceivedByUserName { get; set; }
        public bool Processed { get; set; }
    }
}
