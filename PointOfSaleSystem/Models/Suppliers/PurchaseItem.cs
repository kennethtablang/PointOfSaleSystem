// Models/Suppliers/PurchaseItem.cs (if you keep it)
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Models.Inventory;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PointOfSaleSystem.Models.Suppliers
{
    public class PurchaseItem
    {
        public int Id { get; set; }

        [Required]
        public int PurchaseOrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(0.0001, double.MaxValue)]
        [Precision(18, 2)]
        public decimal CostPerUnit { get; set; }

        // Use decimal for quantity (allows fractional for unit conversions)
        [Required]
        [Precision(18, 2)]
        [Range(0.0001, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Precision(18, 2)]
        public decimal? ReceivedQuantity { get; set; } = 0m;

        public string? Notes { get; set; }

        public PurchaseOrder? PurchaseOrder { get; set; }
        public Product? Product { get; set; }

        [NotMapped]
        public decimal TotalCost => Quantity * CostPerUnit;
    }
}
