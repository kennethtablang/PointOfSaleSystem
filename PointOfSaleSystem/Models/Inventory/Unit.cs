using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Models.Inventory
{
    public class Unit
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;  // e.g., "Piece", "Box"

        public string? Abbreviation { get; set; }         // e.g., "pcs", "bx"

        public string? UnitType { get; set; }             // Optional categorization

        public bool AllowsDecimal { get; set; } = false;  // Enables or disables decimal quantities

        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Product> Products { get; set; } = new List<Product>();

        // Optional (for reverse navigation if needed)
        public ICollection<ProductUnitConversion> FromConversions { get; set; } = new List<ProductUnitConversion>();
        public ICollection<ProductUnitConversion> ToConversions { get; set; } = new List<ProductUnitConversion>();
    }
}
