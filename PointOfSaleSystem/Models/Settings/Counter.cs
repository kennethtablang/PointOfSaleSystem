using PointOfSaleSystem.Models.Auth;
using PointOfSaleSystem.Models.Sales;
using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Models.Settings
{
    public class Counter
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty; // e.g., "Counter 1"

        [MaxLength(100)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // to assign default printer or hardware
        public string? TerminalIdentifier { get; set; } // e.g., Machine name or GUID

        public ICollection<Sale>? Sales { get; set; }

        public ICollection<UserSession>? UserSessions { get; set; } // optional for tracking who used the counter

        public ICollection<XReading>? XReadings { get; set; }
        public ICollection<ZReading>? ZReadings { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
