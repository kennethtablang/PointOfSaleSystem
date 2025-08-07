using PointOfSaleSystem.DTOs.Settings;

namespace PointOfSaleSystem.Interfaces.Settings
{
    public interface ICounterService
    {
        Task<IEnumerable<CounterReadDto>> GetAllAsync();
        Task<CounterReadDto?> GetByIdAsync(int id);
        Task<CounterReadDto> CreateAsync(CounterCreateDto dto);
        Task<bool> UpdateAsync(int id, CounterUpdateDto dto);
        Task<bool> SetActiveAsync(int id, bool isActive);
    }
}
