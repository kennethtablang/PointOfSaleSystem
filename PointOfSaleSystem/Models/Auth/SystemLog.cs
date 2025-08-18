using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PointOfSaleSystem.Models.Auth
{
    [Index(nameof(Timestamp))]
    [Index(nameof(Module))]
    [Index(nameof(ReferenceType), nameof(ReferenceId))]
    public class SystemLog
    {
        public int Id { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // FK to AspNetUsers (ApplicationUser.Id) - nullable for system events
        public string? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        [MaxLength(100)]
        public string Module { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ActionType { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }

        // Consider storing JSON in a text column; length depends on DB provider
        public string? DataBefore { get; set; }
        public string? DataAfter { get; set; }

        [MaxLength(45)]
        public string? IPAddress { get; set; }

        [MaxLength(20)]
        public string? LogLevel { get; set; }
    }
}
