using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Models.Settings
{
    public class BusinessProfile
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string StoreName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string VatRegisteredTIN { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? BIRPermitNumber { get; set; }

        [MaxLength(50)]
        public string? SerialNumber { get; set; }

        [MaxLength(50)]
        public string? MIN { get; set; }  // Machine Identification Number

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? ContactEmail { get; set; }

        [MaxLength(20)]
        public string? ContactPhone { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
