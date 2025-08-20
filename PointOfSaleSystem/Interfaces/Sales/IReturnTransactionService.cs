using PointOfSaleSystem.DTOs.Sales;
using PointOfSaleSystem.Enums;

namespace PointOfSaleSystem.Interfaces.Sales
{
    public interface IReturnTransactionService
    {
        Task<IEnumerable<ReturnTransactionReadDto>> GetAllAsync(int? saleId = null);
        Task<ReturnTransactionReadDto?> GetByIdAsync(int id);
        Task<ReturnTransactionReadDto> CreateAsync(ReturnTransactionCreateDto dto, string returnedByUserId);
        Task<ReturnTransactionReadDto?> UpdateAsync(int id, ReturnTransactionUpdateDto dto);
        Task<ReturnTransactionReadDto?> ChangeStatusAsync(int id, ReturnStatus newStatus, string performedByUserId);
        Task<bool> DeleteAsync(int id);
    }
}
