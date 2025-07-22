using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PointOfSaleSystem.Models.Auth;
using PointOfSaleSystem.Enums;
using PointOfSaleSystem.Models.Settings;

namespace PointOfSaleSystem.Models.Sales
{
    public class Discount
    {
        public int Id { get; set; }

        [Required]
        public int SaleId { get; set; }

        [ForeignKey("SaleId")]
        public Sale Sale { get; set; }

        public int? SaleItemId { get; set; }

        [ForeignKey("SaleItemId")]
        public SaleItem? SaleItem { get; set; }

        // Replaced DiscountType string with FK to DiscountSetting
        [Required]
        public int DiscountSettingId { get; set; }

        [ForeignKey(nameof(DiscountSettingId))]
        public DiscountSetting DiscountSetting { get; set; }

        [Precision(18, 2)]
        public decimal DiscountAmount { get; set; }

        [MaxLength(250)]
        public string? Reason { get; set; } // Justification or remarks

        public DateTime AppliedAt { get; set; } = DateTime.Now;

        public string? AppliedByUserId { get; set; }

        [ForeignKey("AppliedByUserId")]
        public ApplicationUser? AppliedByUser { get; set; }

        public string? ApprovedByUserId { get; set; }

        [ForeignKey("ApprovedByUserId")]
        public ApplicationUser? ApprovedByUser { get; set; }
    }
}
