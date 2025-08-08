using PointOfSaleSystem.DTOs.Inventory;

namespace PointOfSaleSystem.Interfaces.Inventory
{
    public interface IProductUnitConversionService
    {
        Task<IEnumerable<ProductUnitConversionReadDto>> GetAllAsync();
        Task<ProductUnitConversionReadDto?> GetByIdAsync(int id);
        Task<IEnumerable<ProductUnitConversionReadDto>> GetByProductIdAsync(int productId);
        Task<ProductUnitConversionReadDto> CreateAsync(ProductUnitConversionCreateDto dto);
        Task<ProductUnitConversionReadDto> UpdateAsync(ProductUnitConversionUpdateDto dto);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
