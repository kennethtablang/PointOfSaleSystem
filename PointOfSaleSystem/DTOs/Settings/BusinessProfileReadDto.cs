using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Settings
{
    public class BusinessProfileReadDto
    {
        public int Id { get; set; }
        public string StoreName { get; set; } = default!;
        public string VatRegisteredTIN { get; set; } = default!;
        public string? BIRPermitNumber { get; set; }
        public string? SerialNumber { get; set; }
        public string? MIN { get; set; }
        public string? Address { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class BusinessProfileCreateDto
    {
        [Required, MaxLength(150)]
        public string StoreName { get; set; } = default!;

        [Required, MaxLength(20)]
        public string VatRegisteredTIN { get; set; } = default!;

        [MaxLength(100)]
        public string? BIRPermitNumber { get; set; }

        [MaxLength(50)]
        public string? SerialNumber { get; set; }

        [MaxLength(50)]
        public string? MIN { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? ContactEmail { get; set; }

        [MaxLength(20)]
        public string? ContactPhone { get; set; }
    }

    public class BusinessProfileUpdateDto
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string StoreName { get; set; } = default!;

        [Required, MaxLength(20)]
        public string VatRegisteredTIN { get; set; } = default!;

        [MaxLength(100)]
        public string? BIRPermitNumber { get; set; }

        [MaxLength(50)]
        public string? SerialNumber { get; set; }

        [MaxLength(50)]
        public string? MIN { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? ContactEmail { get; set; }

        [MaxLength(20)]
        public string? ContactPhone { get; set; }
    }

}
