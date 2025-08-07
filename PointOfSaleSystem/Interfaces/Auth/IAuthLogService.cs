using PointOfSaleSystem.DTOs.Auth;

namespace PointOfSaleSystem.Interfaces.Auth
{
    public interface IAuthLogService
    {
        Task LogLoginAttemptAsync(LoginAttemptLogDto dto);
        Task LogSystemActionAsync(SystemLogDto dto);
        Task StartUserSessionAsync(UserSessionDto dto);
        Task EndUserSessionAsync(string userId);

        Task<IEnumerable<LoginAttemptLogDto>> GetLoginAttemptsAsync();
        Task<IEnumerable<SystemLogDto>> GetSystemLogsAsync();
        Task<IEnumerable<UserSessionDto>> GetActiveUserSessionsAsync();
    }
}
