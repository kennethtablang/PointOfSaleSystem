using PointOfSaleSystem.Models.Auth;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PointOfSaleSystem.Enums;
using Microsoft.EntityFrameworkCore;

namespace PointOfSaleSystem.Models.Suppliers
{
    [Index(nameof(PurchaseOrderNumber), IsUnique = true)]
    public class PurchaseOrder
    {
        public int Id { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [Required]
        [MaxLength(200)]
        public string PurchaseOrderNumber { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public bool IsReceived { get; set; } = false;

        public string? Remarks { get; set; }

        public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;

        // Optional enhancements
        public DateTime? ExpectedDeliveryDate { get; set; }

        [Range(0, double.MaxValue)]
        [Precision(18, 2)]
        public decimal TotalCost { get; set; }

        // Navigation
        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }

        public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? CreatedByUserId { get; set; }
        [ForeignKey("CreatedByUserId")]
        public ApplicationUser? CreatedByUser { get; set; }

        public ICollection<ReceivedStock> ReceivedStocks { get; set; }

    }
}
