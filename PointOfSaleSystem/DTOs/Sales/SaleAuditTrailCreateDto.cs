using PointOfSaleSystem.Enums;

namespace PointOfSaleSystem.DTOs.Sales
{
    public class SaleAuditTrailCreateDto
    {
        public int SaleId { get; set; }
        public SaleAuditActionType ActionType { get; set; }
        public DateTime? ActionAt { get; set; }
        public string? PerformedByUserId { get; set; }
        public string? Details { get; set; }
    }

    public class SaleAuditTrailReadDto
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public SaleAuditActionType ActionType { get; set; }
        public DateTime ActionAt { get; set; }
        public string? PerformedByUserId { get; set; }
        public string? PerformedByUserName { get; set; }
        public string? Details { get; set; }
    }
}
