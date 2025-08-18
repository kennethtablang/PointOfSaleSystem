using PointOfSaleSystem.DTOs.Inventory;

namespace PointOfSaleSystem.Interfaces.Inventory
{
    public interface IBadOrderService
    {
        Task<IEnumerable<BadOrderReadDto>> GetAllAsync();
        Task<BadOrderReadDto?> GetByIdAsync(int id);
        Task<BadOrderReadDto> CreateAsync(BadOrderCreateDto dto, string reportedByUserId);
        Task<BadOrderReadDto?> UpdateAsync(int id, BadOrderUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
