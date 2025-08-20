using PointOfSaleSystem.DTOs.Sales;

namespace PointOfSaleSystem.Interfaces.Sales
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentReadDto>> GetAllAsync(int? saleId = null);
        Task<PaymentReadDto?> GetByIdAsync(int id);
        Task<PaymentReadDto> CreateAsync(int saleId, PaymentCreateDto dto, string performedByUserId);
        Task<PaymentReadDto?> UpdateAsync(int id, PaymentCreateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<PaymentReadDto>> GetBySaleIdAsync(int saleId);
    }
}
