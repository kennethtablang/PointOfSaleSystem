using PointOfSaleSystem.DTOs.Settings;

namespace PointOfSaleSystem.Interfaces.Settings
{
    public interface IDiscountSettingService
    {
        Task<IEnumerable<DiscountSettingReadDto>> GetAllAsync();
        Task<DiscountSettingReadDto?> GetByIdAsync(int id);
        Task<DiscountSettingReadDto> CreateAsync(DiscountSettingCreateDto dto);
        Task<bool> UpdateAsync(int id, DiscountSettingUpdateDto dto);
        Task<bool> DeactivateAsync(int id);
    }
}
