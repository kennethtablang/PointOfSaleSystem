using PointOfSaleSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Settings
{
    public class VatSettingReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;           // e.g. "VAT"
        public decimal Rate { get; set; }                      // e.g. 12.00
        public TaxType TaxType { get; set; }                   // Vatable, Exempt, etc.
        public bool IsVatInclusive { get; set; }               // true = prices include VAT
        public string? Description { get; set; }               // optional notes
        public bool IsActive { get; set; }                     // whether this rate is currently in use
    }

    public class VatSettingCreateDto
    {
        [Required, MaxLength(50)]
        public string Name { get; set; } = default!;

        [Required, Range(0, 100)]
        public decimal Rate { get; set; }

        [Required]
        public TaxType TaxType { get; set; }

        [Required]
        public bool IsVatInclusive { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class VatSettingUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = default!;

        [Required, Range(0, 100)]
        public decimal Rate { get; set; }

        [Required]
        public TaxType TaxType { get; set; }

        [Required]
        public bool IsVatInclusive { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}
