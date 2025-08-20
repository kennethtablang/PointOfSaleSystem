using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Sales
{
    public class DiscountCreateDto
    {
        [Required]
        public int SaleId { get; set; }

        public int? SaleItemId { get; set; }

        [Required]
        public int DiscountSettingId { get; set; }

        [Required]
        public decimal DiscountAmount { get; set; }

        // snapshot percent applied for history; service may compute/populate this
        public decimal? PercentApplied { get; set; }

        [MaxLength(250)]
        public string? Reason { get; set; }

        public DateTime? AppliedAt { get; set; }

        // optional: applied by user; server will typically set from auth
        public string? AppliedByUserId { get; set; }
    }

    // NEW: Update DTO for Discount
    public class DiscountUpdateDto
    {
        [Required] public int Id { get; set; }

        // you can update amount / reason / approval
        [Required] public decimal DiscountAmount { get; set; }

        // optionally update snapshot percent applied for history if needed
        [Range(0, 100)]
        public decimal? PercentApplied { get; set; }

        [MaxLength(250)]
        public string? Reason { get; set; }

        // optional: approving user (server may set)
        public string? ApprovedByUserId { get; set; }

        public DateTime? AppliedAt { get; set; }
    }

    public class DiscountReadDto
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public int? SaleItemId { get; set; }
        public int DiscountSettingId { get; set; }
        public string? DiscountSettingName { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal PercentApplied { get; set; }
        public string? Reason { get; set; }
        public DateTime AppliedAt { get; set; }
        public string? AppliedByUserId { get; set; }
        public string? AppliedByUserName { get; set; }
        public string? ApprovedByUserId { get; set; }
        public string? ApprovedByUserName { get; set; }
    }
}
