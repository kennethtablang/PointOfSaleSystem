using PointOfSaleSystem.DTOs.Auth;

namespace PointOfSaleSystem.Interfaces.Auth
{
    public interface IUserSessionService
    {
        Task<IEnumerable<UserSessionDto>> GetAllAsync();
        Task<IEnumerable<UserSessionDto>> GetActiveAsync();
        Task<IEnumerable<UserSessionDto>> GetByUserIdAsync(string userId);
    }
}
