using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Inventory.Category
{
    public class CategoryCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }
    }
}
