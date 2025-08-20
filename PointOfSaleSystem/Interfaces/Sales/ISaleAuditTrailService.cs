using PointOfSaleSystem.DTOs.Sales;

namespace PointOfSaleSystem.Interfaces.Sales
{
    public interface ISaleAuditTrailService
    {
        Task<IEnumerable<SaleAuditTrailReadDto>> GetBySaleIdAsync(int saleId);
        Task<SaleAuditTrailReadDto> CreateAsync(SaleAuditTrailCreateDto dto, string performedByUserId);
        Task<IEnumerable<SaleAuditTrailReadDto>> QueryAsync(int? saleId = null, DateTime? from = null, DateTime? to = null);
    }
}
