using PointOfSaleSystem.Models.Auth;
using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Models.Printing
{
    public class BackupLog
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public DateTime BackupDate { get; set; }

        [Required]
        public string ActionByUserId { get; set; } = string.Empty;

        [Required]
        public bool IsRestore { get; set; } = false;

        [MaxLength(300)]
        public string? Notes { get; set; }

        // Navigation
        public ApplicationUser? ActionByUser { get; set; }
    }
}
