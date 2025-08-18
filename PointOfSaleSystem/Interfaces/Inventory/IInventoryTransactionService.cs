using PointOfSaleSystem.DTOs.Inventory;
using PointOfSaleSystem.Enums;

namespace PointOfSaleSystem.Interfaces.Inventory
{
    public interface IInventoryTransactionService
    {
        Task<IEnumerable<InventoryTransactionReadDto>> GetAllAsync(
            int? productId = null,
            InventoryActionType? actionType = null,
            DateTime? from = null,
            DateTime? to = null);
        Task<InventoryTransactionReadDto?> GetByIdAsync(int id);
        Task<InventoryTransactionReadDto> CreateAsync(InventoryTransactionCreateDto dto, string performedByUserId);
        Task<bool> UpdateAsync(int id, InventoryTransactionUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<decimal> GetProductOnHandAsync(int productId);
        Task<ProductStockDto> GetProductStockAsync(int productId);
    }
}
