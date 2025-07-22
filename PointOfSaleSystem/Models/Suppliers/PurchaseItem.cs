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
        [Range(0.01, double.MaxValue)]
        [Precision(18, 2)]
        public decimal CostPerUnit { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public int? ReceivedQuantity { get; set; } = 0;

        public string? Notes { get; set; }

        // Navigation
        public PurchaseOrder? PurchaseOrder { get; set; }
        public Product? Product { get; set; }

        // convenience property(Not mapped to DB)
        [NotMapped]
        public decimal TotalCost => Quantity * CostPerUnit;
    }
}
