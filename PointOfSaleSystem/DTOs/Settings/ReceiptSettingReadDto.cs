using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Settings
{
    public class ReceiptSettingReadDto
    {
        public int Id { get; set; }
        public string? HeaderMessage { get; set; }
        public string? FooterMessage { get; set; }
        public string? LogoUrl { get; set; }
        public string? ReceiptSize { get; set; }
        public bool ShowVatBreakdown { get; set; }
        public bool ShowSerialAndPermitNumber { get; set; }
        public bool ShowItemCode { get; set; }
        public bool IsActive { get; set; }
    }

    public class ReceiptSettingCreateDto
    {
        [MaxLength(200)]
        public string? HeaderMessage { get; set; }

        [MaxLength(200)]
        public string? FooterMessage { get; set; }

        [MaxLength(200)]
        public string? LogoUrl { get; set; }

        [MaxLength(50)]
        public string? ReceiptSize { get; set; } = "58mm";

        public bool ShowVatBreakdown { get; set; } = true;

        public bool ShowSerialAndPermitNumber { get; set; } = true;

        public bool ShowItemCode { get; set; } = true;
    }

    public class ReceiptSettingUpdateDto
    {
        [MaxLength(200)]
        public string? HeaderMessage { get; set; }

        [MaxLength(200)]
        public string? FooterMessage { get; set; }

        [MaxLength(200)]
        public string? LogoUrl { get; set; }

        [MaxLength(50)]
        public string? ReceiptSize { get; set; }

        public bool ShowVatBreakdown { get; set; }

        public bool ShowSerialAndPermitNumber { get; set; }

        public bool ShowItemCode { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
