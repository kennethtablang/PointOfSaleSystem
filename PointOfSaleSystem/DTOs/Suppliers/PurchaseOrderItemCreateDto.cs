using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Suppliers
{
    public class PurchaseOrderItemCreateDto
    {
        [Required] public int ProductId { get; set; }

        // Unit used on PO (e.g., Box). Must be provided.
        [Required] public int UnitId { get; set; }

        [Required]
        [Range(0.0001, double.MaxValue)]
        public decimal QuantityOrdered { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitCost { get; set; } // cost per ordered unit (see assumptions)

        [MaxLength(250)]
        public string? Remarks { get; set; }
    }

    public class PurchaseOrderItemReadDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int UnitId { get; set; }
        public string? UnitName { get; set; }

        public decimal QuantityOrdered { get; set; }
        public decimal QuantityReceived { get; set; }
        public decimal UnitCost { get; set; }
        public string? Remarks { get; set; }

        // convenience
        public decimal RemainingOrdered { get; set; }
        public bool IsClosed { get; set; }
    }

    public class PurchaseOrderItemUpdateDto
    {
        public int? Id { get; set; } // null for new lines

        [Required] public int ProductId { get; set; }
        [Required] public int UnitId { get; set; }

        [Required]
        [Range(0.0001, double.MaxValue)]
        public decimal QuantityOrdered { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitCost { get; set; }

        [MaxLength(250)]
        public string? Remarks { get; set; }
    }
}
