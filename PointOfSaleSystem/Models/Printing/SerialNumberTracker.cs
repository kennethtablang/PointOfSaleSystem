using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Models.Printing
{
    public class SerialNumberTracker
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string SerialStart { get; set; } = string.Empty; // e.g., "AA000001"

        [Required]
        [MaxLength(20)]
        public string SerialEnd { get; set; } = string.Empty; // e.g., "AA001000"

        [Required]
        [MaxLength(20)]
        public string CurrentSerial { get; set; } = string.Empty; // To track where we are in the book

        [MaxLength(100)]
        public string? Remarks { get; set; }

        [Required]
        public DateTime DateIssued { get; set; }

        [Required]
        public bool IsActive { get; set; } = true; // Only one active tracker at a time

        [Required]
        public bool IsDepleted { get; set; } = false; // Mark when full range is used
    }
}
