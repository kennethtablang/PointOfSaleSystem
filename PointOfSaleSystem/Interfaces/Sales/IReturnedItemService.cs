using PointOfSaleSystem.DTOs.Sales;

namespace PointOfSaleSystem.Interfaces.Sales
{
    public interface IReturnedItemService
    {
        Task<IEnumerable<ReturnedItemReadDto>> GetAllAsync(int? returnTransactionId = null);
        Task<ReturnedItemReadDto?> GetByIdAsync(int id);
        Task<ReturnedItemReadDto> CreateAsync(int returnTransactionId, ReturnedItemCreateDto dto);
        Task<ReturnedItemReadDto?> UpdateAsync(int id, ReturnedItemUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
