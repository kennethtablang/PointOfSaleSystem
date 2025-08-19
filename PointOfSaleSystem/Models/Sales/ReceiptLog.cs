using PointOfSaleSystem.Models.Auth;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PointOfSaleSystem.Enums;

namespace PointOfSaleSystem.Models.Sales
{
    public class ReceiptLog
    {
        public int Id { get; set; }

        [Required]
        public int SaleId { get; set; }

        [ForeignKey("SaleId")]
        public Sale Sale { get; set; }

        [Required]
        [MaxLength(50)]
        public string ReceiptNumber { get; set; } = string.Empty;

        public bool IsReprint { get; set; } = false;
        public int PrintCount { get; set; } = 1;
        public DateTime PrintedAt { get; set; } = DateTime.Now;

        [Required]
        public string PrintedByUserId { get; set; }
        [ForeignKey("PrintedByUserId")]
        public ApplicationUser PrintedBy { get; set; }

        [MaxLength(100)]
        public string? TerminalIdentifier { get; set; }

        [MaxLength(250)]
        public string? Remarks { get; set; }

        // New: receipt classification and device info
        public ReceiptType ReceiptType { get; set; } = ReceiptType.Original;

        [MaxLength(200)]
        public string? DeviceIdentifier { get; set; }
    }
}
