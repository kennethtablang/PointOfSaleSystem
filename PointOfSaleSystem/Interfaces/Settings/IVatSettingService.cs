using PointOfSaleSystem.DTOs.Settings;

namespace PointOfSaleSystem.Interfaces.Settings
{
    public interface IVatSettingService
    {
        Task<IEnumerable<VatSettingReadDto>> GetAllAsync();
        Task<VatSettingReadDto?> GetByIdAsync(int id);
        Task<VatSettingReadDto> CreateAsync(VatSettingCreateDto dto);
        Task<bool> UpdateAsync(VatSettingUpdateDto dto);
        Task<bool> SetActiveAsync(int id, bool isActive);
    }
}
