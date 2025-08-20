using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Sales
{
    public class VoidTransactionCreateDto
    {
        [Required]
        public int SaleId { get; set; }

        [MaxLength(250)]
        public string Reason { get; set; } = string.Empty;

        // server will fill from authenticated user normally
        public string? VoidedByUserId { get; set; }

        // optional: approval user (supervisor) if required
        public string? ApprovalUserId { get; set; }

        public bool IsSystemVoid { get; set; } = false;

        [MaxLength(100)]
        public string? TerminalIdentifier { get; set; }

        public DateTime? VoidedAt { get; set; }
    }

    public class VoidTransactionReadDto
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public string? VoidedByUserId { get; set; }
        public string? VoidedByUserName { get; set; }
        public DateTime VoidedAt { get; set; }
        public string? Reason { get; set; }
        public string? TerminalIdentifier { get; set; }
        public string? OriginalCashierUserId { get; set; }
        public string? OriginalCashierName { get; set; }
        public string? ApprovalUserId { get; set; }
        public string? ApprovalUserName { get; set; }
        public bool IsSystemVoid { get; set; }
    }
}
