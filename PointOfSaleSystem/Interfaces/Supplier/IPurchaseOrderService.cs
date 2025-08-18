using PointOfSaleSystem.DTOs.Suppliers;

namespace PointOfSaleSystem.Interfaces.Supplier
{
    public interface IPurchaseOrderService
    {
        // Purchase Orders
        Task<IEnumerable<PurchaseOrderReadDto>> GetAllAsync();
        Task<PurchaseOrderReadDto?> GetByIdAsync(int id);
        Task<PurchaseOrderReadDto> CreateAsync(PurchaseOrderCreateDto dto, string createdByUserId);
        Task<PurchaseOrderReadDto> UpdateAsync(PurchaseOrderUpdateDto dto, string updatedByUserId);
        Task<bool> DeleteAsync(int id); // returns true if deleted

        Task<ReceivedStockReadDto> ReceiveStockAsync(ReceiveStockCreateDto dto, string receivedByUserId);
        Task<bool> DeleteReceivedStockAsync(int receivedStockId);
        Task<bool> RemoveItemByIdAsync(int purchaseOrderItemId);

        Task<IEnumerable<PurchaseOrderReadDto>> GetPendingReceivingsAsync();
        Task PostReceivedToInventoryAsync(int purchaseOrderId, string processedByUserId);

    }
}
