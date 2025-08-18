using PointOfSaleSystem.DTOs.Inventory;

namespace PointOfSaleSystem.Interfaces.Inventory
{
    public interface IStockAdjustmentService
    {
        Task<IEnumerable<StockAdjustmentListDto>> GetAllAsync();
        Task<StockAdjustmentReadDto?> GetByIdAsync(int id);
        Task<StockAdjustmentReadDto> CreateAsync(StockAdjustmentCreateDto dto, string userId);
        Task<StockAdjustmentReadDto?> UpdateAsync(int id, StockAdjustmentUpdateDto dto, string userId);
        Task<bool> DeleteAsync(int id);
    }
}
