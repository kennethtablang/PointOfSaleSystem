using PointOfSaleSystem.DTOs.Auth;

namespace PointOfSaleSystem.Interfaces.Auth
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(RegisterDto dto);
        Task<LoginResponseDto?> LoginAsync(LoginDto dto);
    }
}
