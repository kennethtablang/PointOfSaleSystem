using PointOfSaleSystem.DTOs.Sales;

namespace PointOfSaleSystem.Interfaces.Sales
{
    public interface IVoidTransactionService
    {
        Task<IEnumerable<VoidTransactionReadDto>> GetAllAsync(int? saleId = null);
        Task<VoidTransactionReadDto?> GetByIdAsync(int id);
        Task<VoidTransactionReadDto> CreateAsync(VoidTransactionCreateDto dto, string performedByUserId);
        Task<VoidTransactionReadDto?> ApproveAsync(int voidTransactionId, string approvalUserId, bool approved, string? approvalNotes = null);
        Task<bool> DeleteAsync(int id);
    }
}
