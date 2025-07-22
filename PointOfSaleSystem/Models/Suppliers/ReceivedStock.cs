using PointOfSaleSystem.Models.Auth;
using PointOfSaleSystem.Models.Inventory;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PointOfSaleSystem.Models.Suppliers
{
    public class ReceivedStock
    {
        public int Id { get; set; }

        [Required]
        public int PurchaseOrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int QuantityReceived { get; set; }

        public DateTime ReceivedDate { get; set; } = DateTime.Now;

        [MaxLength(200)]
        public string? ReferenceNumber { get; set; } // e.g., delivery receipt number

        public string? ReceivedByUserId { get; set; }

        public string? Notes { get; set; }

        // Optional link to inventory movement
        public int? InventoryTransactionId { get; set; }

        [ForeignKey("InventoryTransactionId")]
        public InventoryTransaction? InventoryTransaction { get; set; }

        // Navigation
        public PurchaseOrder? PurchaseOrder { get; set; }
        public Product? Product { get; set; }

        [ForeignKey("ReceivedByUserId")]
        public ApplicationUser? ReceivedByUser { get; set; }
    }
}
