using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Models.Inventory;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Models.Suppliers
{
    [Comment("Purchase order line items")]
    [Index(nameof(PurchaseOrderId), nameof(ProductId))]
    public class PurchaseOrderItem
    {
        public int Id { get; set; }

        [Required]
        public int PurchaseOrderId { get; set; }
        public PurchaseOrder? PurchaseOrder { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int UnitId { get; set; }
        public Unit? Unit { get; set; }

        [Required]
        [Precision(18, 2)]
        [Range(0.0001, double.MaxValue)]
        public decimal QuantityOrdered { get; set; }

        [Precision(18, 2)]
        public decimal QuantityReceived { get; set; } = 0m;

        [Precision(18, 2)]
        public decimal UnitCost { get; set; }

        [MaxLength(250)]
        public string? Remarks { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [NotMapped]
        public decimal RemainingOrdered => Math.Max(0, QuantityOrdered - QuantityReceived);

        [NotMapped]
        public bool IsClosed => QuantityReceived >= QuantityOrdered;
    }
}
