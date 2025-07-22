using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Models.Settings
{
    public class ReceiptSetting
    {
        public int Id { get; set; }

        [MaxLength(200)]
        public string? HeaderMessage { get; set; }

        [MaxLength(200)]
        public string? FooterMessage { get; set; }

        [MaxLength(200)]
        public string? LogoUrl { get; set; }  // Path or base64 for printed receipts

        [MaxLength(50)]
        public string? ReceiptSize { get; set; } = "58mm"; // or "80mm"

        public bool ShowVatBreakdown { get; set; } = true;

        public bool ShowSerialAndPermitNumber { get; set; } = true;

        public bool ShowItemCode { get; set; } = true;

        [Required]
        public bool IsActive { get; set; } = true;
    }
}
