using PointOfSaleSystem.DTOs.Auth;
using PointOfSaleSystem.Models.Auth;

namespace PointOfSaleSystem.Interfaces.Auth
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(RegisterDto dto);
        Task<LoginResponseDto?> LoginAsync(LoginDto dto);

        //User Management
        Task<bool> CreateUserAsync(UserCreateDto dto);
        Task<IEnumerable<UserReadDto>> GetAllAsync();
        Task<UserReadDto?> GetByIdAsync(string id);
        Task<bool> UpdateAsync(UserUpdateDto dto);
        Task<bool> DeactivateAsync(string id);
        Task<ApplicationUser?> FindByEmailAsync(string email);
    }
}
