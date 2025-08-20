using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Sales
{
    public class ReceiptLogCreateDto
    {
        [Required]
        public int SaleId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ReceiptNumber { get; set; } = string.Empty;

        public bool IsReprint { get; set; } = false;

        public int PrintCount { get; set; } = 1;

        public DateTime? PrintedAt { get; set; }

        [Required]
        public string PrintedByUserId { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? TerminalIdentifier { get; set; }

        [MaxLength(250)]
        public string? Remarks { get; set; }

        public Enums.ReceiptType ReceiptType { get; set; } = Enums.ReceiptType.Original;

        [MaxLength(200)]
        public string? DeviceIdentifier { get; set; }
    }

    public class ReceiptLogReadDto
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public bool IsReprint { get; set; }
        public int PrintCount { get; set; }
        public DateTime PrintedAt { get; set; }
        public string PrintedByUserId { get; set; } = string.Empty;
        public string? PrintedByUserName { get; set; }
        public string? TerminalIdentifier { get; set; }
        public string? Remarks { get; set; }
        public Enums.ReceiptType ReceiptType { get; set; }
        public string? DeviceIdentifier { get; set; }
    }
}
