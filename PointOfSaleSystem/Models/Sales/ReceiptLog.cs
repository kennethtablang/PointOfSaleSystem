using PointOfSaleSystem.Models.Auth;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Models.Sales
{
    public class ReceiptLog
    {
        public int Id { get; set; }

        [Required]
        public int SaleId { get; set; }

        [ForeignKey("SaleId")]
        public Sale Sale { get; set; }

        [Required]
        [MaxLength(50)]
        public string ReceiptNumber { get; set; } = string.Empty;

        public bool IsReprint { get; set; } = false;

        public int PrintCount { get; set; } = 1;

        public DateTime PrintedAt { get; set; } = DateTime.Now;

        [Required]
        public string PrintedByUserId { get; set; }

        [ForeignKey("PrintedByUserId")]
        public ApplicationUser PrintedBy { get; set; }

        [MaxLength(100)]
        public string? TerminalIdentifier { get; set; } // logs which counter/machine

        [MaxLength(250)]
        public string? Remarks { get; set; }
    }
}
