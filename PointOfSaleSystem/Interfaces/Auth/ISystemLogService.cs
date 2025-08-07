using PointOfSaleSystem.DTOs.Auth;

namespace PointOfSaleSystem.Interfaces.Auth
{
    public interface ISystemLogService
    {
        Task<IEnumerable<SystemLogDto>> GetAllAsync();
        Task<SystemLogDto?> GetByIdAsync(int id);
    }
}
