using PointOfSaleSystem.Models.Auth;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PointOfSaleSystem.Enums;

namespace PointOfSaleSystem.Models.Sales
{
    public class ReturnTransaction
    {
        public int Id { get; set; }

        [Required]
        public int OriginalSaleId { get; set; }
        [ForeignKey("OriginalSaleId")]
        public Sale OriginalSale { get; set; }

        [Required]
        public DateTime ReturnDate { get; set; } = DateTime.Now;

        [Required]
        public string ReturnedByUserId { get; set; }
        public ApplicationUser ReturnedBy { get; set; }

        [MaxLength(250)]
        public string Reason { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalRefundAmount { get; set; }

        [MaxLength(100)]
        public string? TerminalIdentifier { get; set; }

        public ICollection<ReturnedItem> Items { get; set; } = new List<ReturnedItem>();

        // New: status of the return flow
        public ReturnStatus Status { get; set; } = ReturnStatus.Pending;

        // New: chosen method for refund (cash, card, store credit, etc.)
        public RefundMethod RefundMethod { get; set; } = RefundMethod.Cash;
    }
}
