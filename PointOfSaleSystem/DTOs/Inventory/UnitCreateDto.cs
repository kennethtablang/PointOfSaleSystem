using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Inventory
{
    public class UnitCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Abbreviation { get; set; }

        [MaxLength(50)]
        public string? UnitType { get; set; }

        public bool AllowsDecimal { get; set; } = false;
    }

    public class UnitUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Abbreviation { get; set; }

        [MaxLength(50)]
        public string? UnitType { get; set; }

        public bool AllowsDecimal { get; set; } = false;

        public bool IsActive { get; set; } = true;
    }

    public class UnitViewDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Abbreviation { get; set; }
        public string? UnitType { get; set; }
        public bool AllowsDecimal { get; set; }
        public bool IsActive { get; set; }
    }
}
