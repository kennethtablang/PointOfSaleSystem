using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Settings
{
    public class DiscountSettingReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool RequiresApproval { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class DiscountSettingCreateDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal DiscountPercent { get; set; }

        [Required]
        public bool RequiresApproval { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }
    }

    public class DiscountSettingUpdateDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal DiscountPercent { get; set; }

        [Required]
        public bool RequiresApproval { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }

}
