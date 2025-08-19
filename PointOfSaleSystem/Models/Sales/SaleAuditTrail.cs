using PointOfSaleSystem.Enums;
using PointOfSaleSystem.Models.Auth;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Models.Sales
{
    public class SaleAuditTrail
    {
        public int Id { get; set; }

        [Required]
        public int SaleId { get; set; }
        [ForeignKey("SaleId")]
        public Sale Sale { get; set; }

        [Required]
        public SaleAuditActionType ActionType { get; set; }

        [Required]
        public DateTime ActionAt { get; set; } = DateTime.UtcNow;

        public string PerformedByUserId { get; set; }
        [ForeignKey("PerformedByUserId")]
        public ApplicationUser PerformedBy { get; set; }

        // JSON or free text details (what changed, old/new values, reason, etc.)
        [MaxLength(2000)]
        public string? Details { get; set; }
    }
}
