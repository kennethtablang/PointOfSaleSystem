using PointOfSaleSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Sales
{
    public class ReturnTransactionCreateDto
    {
        [Required]
        public int OriginalSaleId { get; set; }

        // server will typically use authenticated user; optional here for flexibility
        public string? ReturnedByUserId { get; set; }

        public DateTime? ReturnDate { get; set; }

        [MaxLength(250)]
        public string? Reason { get; set; }

        public RefundMethod RefundMethod { get; set; } = RefundMethod.Cash;

        // items
        [Required]
        public List<ReturnedItemCreateDto> Items { get; set; } = new();
    }

    public class ReturnTransactionUpdateDto
    {
        [Required]
        public int Id { get; set; }

        public DateTime? ReturnDate { get; set; }

        [MaxLength(250)]
        public string? Reason { get; set; }

        public RefundMethod? RefundMethod { get; set; }

        public ReturnStatus? Status { get; set; }

        public string? TerminalIdentifier { get; set; }
    }

    public class ReturnTransactionReadDto
    {
        public int Id { get; set; }
        public int OriginalSaleId { get; set; }
        public DateTime ReturnDate { get; set; }
        public string? ReturnedByUserId { get; set; }
        public string? ReturnedByUserName { get; set; }
        public string? Reason { get; set; }
        public decimal? TotalRefundAmount { get; set; }
        public string? TerminalIdentifier { get; set; }
        public ReturnStatus Status { get; set; }
        public RefundMethod RefundMethod { get; set; }
        public List<ReturnedItemReadDto> Items { get; set; } = new();
    }
}
