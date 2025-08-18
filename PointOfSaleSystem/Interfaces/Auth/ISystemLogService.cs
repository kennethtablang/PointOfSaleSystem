using PointOfSaleSystem.DTOs.Auth;

namespace PointOfSaleSystem.Interfaces.Auth
{
    public interface ISystemLogService
    {
        Task<IEnumerable<SystemLogDto>> GetSystemLogsAsync();
        Task<SystemLogDto?> GetSystemLogByIdAsync(int id);
        Task LogSystemActionAsync(SystemLogDto dto);

    }
}
