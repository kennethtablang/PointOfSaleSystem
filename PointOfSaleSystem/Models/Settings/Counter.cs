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
        public string Name { get; set; } = string.Empty; // Counter 1

        [MaxLength(100)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // to assign default printer or hardware
        public string? TerminalIdentifier { get; set; }

        public ICollection<Sale>? Sales { get; set; }

        public ICollection<UserSession>? UserSessions { get; set; }

        public ICollection<XReading>? XReadings { get; set; }
        public ICollection<ZReading>? ZReadings { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
