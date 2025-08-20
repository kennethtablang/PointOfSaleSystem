using PointOfSaleSystem.DTOs.Sales;

namespace PointOfSaleSystem.Interfaces.Sales
{
    public interface ISaleItemService
    {
        Task<IEnumerable<SaleItemReadDto>> GetBySaleIdAsync(int saleId);
        Task<SaleItemReadDto?> GetByIdAsync(int id);
        Task<SaleItemReadDto> CreateAsync(int saleId, SaleItemCreateDto dto, string performedByUserId);
        Task<SaleItemReadDto?> UpdateAsync(int id, SaleItemCreateDto dto, string performedByUserId);
        Task<bool> DeleteAsync(int id, string performedByUserId);
    }
}
