using PointOfSaleSystem.DTOs.Inventory;

namespace PointOfSaleSystem.Interfaces.Inventory
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryViewDto>> GetAllAsync();
        Task<CategoryViewDto?> GetByIdAsync(int id);
        Task<CategoryViewDto> CreateAsync(CategoryCreateDto dto);
        Task<bool> UpdateAsync(CategoryUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
