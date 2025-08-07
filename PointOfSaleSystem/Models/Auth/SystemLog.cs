using System.ComponentModel.DataAnnotations.Schema;

namespace PointOfSaleSystem.Models.Auth
{
    public class SystemLog
    {
        public int Id { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [ForeignKey("User")]
        public string? UserId { get; set; }

        public string Module { get; set; } = string.Empty;       // e.g., "Product", "Sale"
        public string ActionType { get; set; } = string.Empty;   // e.g., "CREATE", "DELETE"
        public string Description { get; set; } = string.Empty;

        public string? DataBefore { get; set; } // serialized JSON
        public string? DataAfter { get; set; }  // serialized JSON

        public string? IPAddress { get; set; }

        public ApplicationUser? User { get; set; }
    }
}
