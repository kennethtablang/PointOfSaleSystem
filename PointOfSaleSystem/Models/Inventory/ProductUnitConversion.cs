using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PointOfSaleSystem.Models.Inventory
{
    public class ProductUnitConversion
    {
        public int Id { get; set; }

        // The product this conversion is linked to
        [Required]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        // From unit (e.g., Box)
        [Required]
        public int FromUnitId { get; set; }
        public Unit? FromUnit { get; set; }

        // To unit (e.g., Piece)
        [Required]
        public int ToUnitId { get; set; }
        public Unit? ToUnit { get; set; }

        // Conversion multiplier (e.g., 1 Box = 24 Pieces)
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Conversion rate must be greater than 0.")]
        [Precision(18, 2)]
        public decimal ConversionRate { get; set; }

        // Optional note or description
        [MaxLength(200)]
        public string? Notes { get; set; }
    }
}
