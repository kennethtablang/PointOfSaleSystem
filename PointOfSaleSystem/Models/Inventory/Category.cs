using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Models.Inventory
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
