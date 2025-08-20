using PointOfSaleSystem.DTOs.Sales;

namespace PointOfSaleSystem.Interfaces.Sales
{
    public interface IReceiptLogService
    {
        Task<IEnumerable<ReceiptLogReadDto>> GetAllAsync(int? saleId = null);
        Task<ReceiptLogReadDto?> GetByIdAsync(int id);
        Task<ReceiptLogReadDto> CreateAsync(ReceiptLogCreateDto dto, string printedByUserId);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ReceiptLogReadDto>> GetBySaleIdAsync(int saleId);
    }
}
