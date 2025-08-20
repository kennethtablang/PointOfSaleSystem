using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Sales
{
    // item DTOs used inside ReturnTransaction create/read
    public class ReturnedItemCreateDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        public string? Remarks { get; set; }
    }

    public class ReturnedItemUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        public string? Remarks { get; set; }
    }

    public class ReturnedItemReadDto
    {
        public int Id { get; set; }
        public int ReturnTransactionId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalRefundAmount { get; set; }
        public string? Remarks { get; set; }
    }
}
