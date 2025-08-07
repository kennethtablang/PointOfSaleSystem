using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PointOfSaleSystem.Models.Inventory;

namespace PointOfSaleSystem.Models.Sales
{
    public class ReturnedItem
    {
        public int Id { get; set; }

        [Required]
        public int ReturnTransactionId { get; set; }

        [ForeignKey("ReturnTransactionId")]
        public ReturnTransaction ReturnTransaction { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }

        [Precision(18, 2)]
        public decimal TotalRefundAmount => UnitPrice * Quantity;
    }
}
