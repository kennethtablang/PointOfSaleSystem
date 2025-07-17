using PointOfSaleSystem.DTOs.Inventory.Unit;

namespace PointOfSaleSystem.Interfaces.Inventory
{
    public interface IUnitService
    {
        Task<IEnumerable<UnitViewDto>> GetAllAsync();
        Task<UnitViewDto?> GetByIdAsync(int id);
        Task<UnitViewDto> CreateAsync(UnitCreateDto dto);
        Task<bool> UpdateAsync(UnitUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
