using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Models.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PointOfSaleSystem.Models.Suppliers
{
    public class ReceivedStock
    {
        public int Id { get; set; }

        [Required]
        public int PurchaseOrderId { get; set; }
        public PurchaseOrder? PurchaseOrder { get; set; }

        [Required]
        public int PurchaseOrderItemId { get; set; }
        public PurchaseOrderItem? PurchaseOrderItem { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Precision(18, 2)]
        [Range(0.0001, double.MaxValue)]
        public decimal QuantityReceived { get; set; }

        public DateTime ReceivedDate { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string? ReferenceNumber { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public bool Processed { get; set; } = false;

        public string? ReceivedByUserId { get; set; }
        [ForeignKey("ReceivedByUserId")]
        public ApplicationUser? ReceivedByUser { get; set; }
    }
}
