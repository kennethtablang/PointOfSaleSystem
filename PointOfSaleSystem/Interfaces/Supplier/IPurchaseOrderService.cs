using PointOfSaleSystem.DTOs.Suppliers;

namespace PointOfSaleSystem.Interfaces.Supplier
{
    public interface IPurchaseOrderService
    {
        // Purchase Orders
        Task<IEnumerable<PurchaseOrderReadDto>> GetAllAsync();
        Task<PurchaseOrderReadDto?> GetByIdAsync(int id);
        Task<PurchaseOrderReadDto> CreateAsync(PurchaseOrderCreateDto dto, string userId);
        Task<bool> UpdateAsync(int id, PurchaseOrderUpdateDto dto);
        Task<bool> DeleteAsync(int id);

        // Purchase Items (related to PO)
        Task<PurchaseItemReadDto> AddPurchaseItemAsync(int purchaseOrderId, PurchaseItemCreateDto dto);
        Task<bool> UpdatePurchaseItemAsync(int id, PurchaseItemUpdateDto dto);
        Task<bool> RemovePurchaseItemAsync(int id);

        // Received Stocks (related to PO)
        Task<ReceivedStockReadDto> AddReceivedStockAsync(ReceivedStockCreateDto dto, string userId);
        Task<bool> DeleteReceivedStockAsync(int id);
    }
}
