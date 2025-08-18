using PointOfSaleSystem.Models.Auth;
using PointOfSaleSystem.Models.Suppliers;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PointOfSaleSystem.Models.Inventory
{
    public class StockReceive
    {
        public int Id { get; set; }

        // Link to the PO that this receive pertains to
        [Required]
        public int PurchaseOrderId { get; set; }
        public PurchaseOrder? PurchaseOrder { get; set; }

        [Required]
        public DateTime ReceivedDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string ReceivedByUserId { get; set; } = string.Empty;
        public ApplicationUser? ReceivedByUser { get; set; }

        [MaxLength(250)]
        public string? ReferenceNumber { get; set; } // e.g., GRN number

        [MaxLength(500)]
        public string? Remarks { get; set; }

        // Navigation: the lines/items for this receive
        public ICollection<StockReceiveItem> Items { get; set; } = new List<StockReceiveItem>();
    }
}
