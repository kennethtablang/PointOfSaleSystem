using PointOfSaleSystem.Models.Auth;
using PointOfSaleSystem.Models.Sales;
using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Models.Printing
{
    public class ReprintLog
    {
        public int Id { get; set; }

        [Required]
        public int SaleId { get; set; }

        [Required]
        public string ReprintedByUserId { get; set; } = string.Empty;

        [Required]
        public DateTime ReprintedAt { get; set; }

        [Required]
        [MaxLength(250)]
        public string Reason { get; set; } = string.Empty;

        public int ReprintCount { get; set; } = 1;

        // Navigation
        public Sale? Sale { get; set; }

        public ApplicationUser? ReprintedByUser { get; set; }
    }
}
