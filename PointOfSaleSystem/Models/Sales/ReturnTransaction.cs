using PointOfSaleSystem.Models.Auth;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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

        [ForeignKey("ReturnedByUserId")]
        public ApplicationUser ReturnedBy { get; set; }

        [MaxLength(250)]
        public string Reason { get; set; } = string.Empty;

        // Optional: Total refund amount (computed in backend, stored for history)
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalRefundAmount { get; set; }

        // Optional: Terminal where the return was processed
        [MaxLength(100)]
        public string? TerminalIdentifier { get; set; }

        public ICollection<ReturnedItem> Items { get; set; } = new List<ReturnedItem>();
    }
}
