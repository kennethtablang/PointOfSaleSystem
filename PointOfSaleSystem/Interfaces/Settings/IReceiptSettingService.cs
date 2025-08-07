using PointOfSaleSystem.DTOs.Settings;

namespace PointOfSaleSystem.Interfaces.Settings
{
    public interface IReceiptSettingService
    {
        Task<IEnumerable<ReceiptSettingReadDto>> GetAllAsync();
        Task<ReceiptSettingReadDto?> GetByIdAsync(int id);
        Task<ReceiptSettingReadDto> CreateAsync(ReceiptSettingCreateDto dto);
        Task<bool> UpdateAsync(int id, ReceiptSettingUpdateDto dto);
        Task<bool> SetActiveAsync(int id, bool isActive);
    }
}
