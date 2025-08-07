using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Models.Settings
{
    public class VatSetting
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [Range(0, 100)]
        [Precision(18, 2)]
        public decimal Rate { get; set; } = 12; // Default 12%

        [Required]
        public TaxType TaxType { get; set; } = TaxType.Vatable;

        [Required]
        public bool IsVatInclusive { get; set; } = true; // true = price includes VAT

        [MaxLength(200)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
