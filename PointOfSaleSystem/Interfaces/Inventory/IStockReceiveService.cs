using PointOfSaleSystem.DTOs.Inventory;

namespace PointOfSaleSystem.Interfaces.Inventory
{
    public interface IStockReceiveService
    {
        Task<IEnumerable<StockReceiveReadDto>> GetAllAsync();
        Task<StockReceiveReadDto?> GetByIdAsync(int id);
        Task<IEnumerable<StockReceiveReadDto>> GetByPurchaseOrderIdAsync(int purchaseOrderId);
        Task<StockReceiveReadDto> CreateFromReceivedStocksAsync(int purchaseOrderId, string processedByUserId, bool allowOverReceive = false);

        Task DeleteAsync(int stockReceiveId);
        Task<bool> ExistsAsync(int id);
    }
}
