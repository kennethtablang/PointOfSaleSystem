using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Inventory.Unit
{
    public class UnitCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }

    public class UnitUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }

    public class UnitViewDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
