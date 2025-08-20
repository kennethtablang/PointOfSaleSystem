using PointOfSaleSystem.DTOs.Sales;

namespace PointOfSaleSystem.Interfaces.Sales
{
    public interface IDiscountService
    {
        Task<IEnumerable<DiscountReadDto>> GetAllAsync(int? saleId = null, int? saleItemId = null);
        Task<DiscountReadDto?> GetByIdAsync(int id);

        Task<DiscountReadDto> CreateAsync(DiscountCreateDto dto, string appliedByUserId);
        Task<DiscountReadDto?> UpdateAsync(int id, DiscountUpdateDto dto);
        Task<bool> DeleteAsync(int id);

        // convenience: get discounts for a sale
        Task<IEnumerable<DiscountReadDto>> GetBySaleIdAsync(int saleId);
    }
}
