using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Inventory
{
    public class BadOrderCreateDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 999999, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        [Required]
        [MaxLength(100)]
        public string Reason { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Remarks { get; set; }

        [Required]
        public DateTime BadOrderDate { get; set; } = DateTime.Now;

        public string? ReportedByUserId { get; set; } = string.Empty;
    }

    public class BadOrderUpdateDto
    {
        [Required]
        [Range(1, 999999, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        [Required]
        [MaxLength(100)]
        public string Reason { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Remarks { get; set; }

        [Required]
        public DateTime BadOrderDate { get; set; } = DateTime.Now;
    }

    public class BadOrderReadDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public DateTime BadOrderDate { get; set; }

        public string ReportedByUserId { get; set; } = string.Empty;
        public string ReportedByUserName { get; set; } = string.Empty;

        // Newly exposed optional/audit fields
        public int? InventoryTransactionId { get; set; }
        public bool IsSystemGenerated { get; set; } = false;
    }
}
