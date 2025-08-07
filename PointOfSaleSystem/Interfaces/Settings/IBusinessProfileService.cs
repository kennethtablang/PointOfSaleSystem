using PointOfSaleSystem.DTOs.Settings;

namespace PointOfSaleSystem.Interfaces.Settings
{
    public interface IBusinessProfileService
    {
        Task<BusinessProfileReadDto?> GetAsync();
        Task<BusinessProfileReadDto> CreateAsync(BusinessProfileCreateDto dto);
        Task<bool> UpdateAsync(BusinessProfileUpdateDto dto);
    }
}
