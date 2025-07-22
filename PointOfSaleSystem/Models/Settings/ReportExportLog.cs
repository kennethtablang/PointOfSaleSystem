using PointOfSaleSystem.Models.Auth;
using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Models.Settings
{
    public class ReportExportLog
    {
        public int Id { get; set; }

        [Required]
        public DateTime ExportedAt { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(100)]
        public string ReportName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string ExportFormat { get; set; } = "PDF"; // or CSV, XLSX, etc.

        [MaxLength(500)]
        public string? Parameters { get; set; } // e.g. date range, filters used

        public string? ExportedByUserId { get; set; }

        // Navigation
        public ApplicationUser? ExportedByUser { get; set; }
    }
}
