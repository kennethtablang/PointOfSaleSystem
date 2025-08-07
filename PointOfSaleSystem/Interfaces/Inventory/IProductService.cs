using PointOfSaleSystem.DTOs.Inventory;

namespace PointOfSaleSystem.Interfaces.Inventory
{
    public interface IProductService
    {
        Task<IEnumerable<ProductViewDto>> GetAllAsync();
        Task<ProductViewDto?> GetByIdAsync(int id);
        Task<ProductViewDto> CreateAsync(ProductCreateDto dto);
        Task<bool> UpdateAsync(ProductUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
