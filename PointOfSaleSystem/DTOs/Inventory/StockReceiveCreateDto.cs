using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Inventory
{
    public class StockReceiveCreateDto
    {
        [Required]
        public int PurchaseOrderId { get; set; }

        [Required]
        public string ReceivedByUserId { get; set; } = string.Empty;

        public DateTime? ReceivedDate { get; set; }

        [MaxLength(100)]
        public string? ReferenceNumber { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one receive item is required")]
        public List<StockReceiveItemCreateDto> Items { get; set; } = new List<StockReceiveItemCreateDto>();

        public bool AllowOverReceive { get; set; } = false;
    }

    public class StockReceiveReadDto
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; } = string.Empty;

        public DateTime ReceivedDate { get; set; }
        public string ReceivedByUserId { get; set; } = string.Empty;
        public string? ReceivedByUserName { get; set; }

        public string? ReferenceNumber { get; set; }
        public string? Remarks { get; set; }

        public List<StockReceiveItemReadDto> Items { get; set; } = new List<StockReceiveItemReadDto>();
    }

    public class StockReceiveUpdateDto
    {
        [Required]
        public int Id { get; set; }

        public DateTime? ReceivedDate { get; set; }

        [MaxLength(100)]
        public string? ReferenceNumber { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }
    }
}
