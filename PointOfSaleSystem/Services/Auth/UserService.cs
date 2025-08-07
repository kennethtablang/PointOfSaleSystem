using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PointOfSaleSystem.DTOs.Auth;
using PointOfSaleSystem.Enums;
using PointOfSaleSystem.Interfaces.Auth;
using PointOfSaleSystem.Models.Auth;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PointOfSaleSystem.Services.Auth
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null || !user.IsActive)
                return null;

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

            if (!result.Succeeded)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new LoginResponseDto
            {
                Token = GenerateJwtToken(user, roles.FirstOrDefault() ?? "Cashier"),
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? "Cashier",
                FullName = user.FullName
            };
        }

        private string GenerateJwtToken(ApplicationUser user, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? throw new Exception("Missing JWT Key")));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                FirstName = dto.FirstName,
                MiddleName = dto.MiddleName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.Email,
                IsActive = true,
                Role = Enum.TryParse<UserRole>(dto.Role, true, out var parsedRole) ? parsedRole : UserRole.Cashier
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return false;

            await _userManager.AddToRoleAsync(user, dto.Role);
            return true;
        }



        public async Task<IEnumerable<UserReadDto>> GetAllAsync()
        {
            var users = _userManager.Users.Where(u => u.IsActive).ToList();
            return _mapper.Map<IEnumerable<UserReadDto>>(users);
        }

        public async Task<UserReadDto?> GetByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || !user.IsActive) return null;

            return _mapper.Map<UserReadDto>(user);
        }

        public async Task<bool> UpdateAsync(UserUpdateDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.Id!);
            if (user == null || !user.IsActive) return false;

            _mapper.Map(dto, user);

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeactivateAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            user.IsActive = false;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> CreateUserAsync(UserCreateDto dto)
        {
            // Use AutoMapper instead of manual property assignment
            var user = _mapper.Map<ApplicationUser>(dto);

            // 1. Create the user (password comes from dto.Password)
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return false;

            // 2. Assign the role (dto.Role is an enum, so we .ToString() it)
            var roleResult = await _userManager.AddToRoleAsync(user, dto.Role.ToString());
            return roleResult.Succeeded;
        }

        public async Task<ApplicationUser?> FindByEmailAsync(string email)
        {
            // This uses ASP.NET Identity to find the user by email
            return await _userManager.FindByEmailAsync(email);
        }
    }
}
