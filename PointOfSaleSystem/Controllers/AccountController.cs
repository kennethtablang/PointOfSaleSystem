using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSystem.DTOs.Auth;
using PointOfSaleSystem.Interfaces.Auth;
using System.Security.Claims;
using static PointOfSaleSystem.Helpers.TimeHelper;

namespace PointOfSaleSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthLogService _authLogService;

        public AccountController(IUserService userService, IAuthLogService authLogService  )
        {
            _userService = userService;
            _authLogService = authLogService;
        }

        // ---------------------
        // Authentication
        // ---------------------

        /// <summary>
        /// Registers a new user (self-registration or Admin-created)
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _userService.RegisterAsync(dto);
            if (!success)
            {
                // Log failed registration attempt as a system action
                await _authLogService.LogSystemActionAsync(new SystemLogDto
                {
                    UserId = null,
                    Module = "Auth",
                    ActionType = "REGISTER_FAILED",
                    Description = $"Registration failed for {dto.Email}",
                    Timestamp = NowPhilippines(),
                    IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                });

                return BadRequest(new { message = "Registration failed. User may already exist or invalid role." });
            }

            // Log successful registration as a system action
            await _authLogService.LogSystemActionAsync(new SystemLogDto
            {
                UserId = null,
                Module = "Auth",
                ActionType = "REGISTER_SUCCESS",
                Description = $"New user registered: {dto.Email}",
                Timestamp = NowPhilippines(),
                IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            return Ok(new { message = "User registered successfully." });
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1. Authenticate and get your token/response
            var loginResult = await _userService.LoginAsync(dto);
            if (loginResult == null)
            {
                await _authLogService.LogLoginAttemptAsync(new LoginAttemptLogDto
                {
                    UsernameOrEmail = dto.Email,
                    AttemptedAt = NowPhilippines(),
                    WasSuccessful = false,
                    FailureReason = "Invalid credentials",
                    IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    TerminalName = Environment.MachineName
                });
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // 2. Lookup the actual ApplicationUser to get its Id
            //    (Add this method to your IUserService if you don't already have it)
            var userEntity = await _userService.FindByEmailAsync(dto.Email);
            if (userEntity == null)
            {
                // Unlikely, but guard just in case
                return StatusCode(500, "User record not found after successful login.");
            }

            // 3. Log the successful login attempt
            await _authLogService.LogLoginAttemptAsync(new LoginAttemptLogDto
            {
                UsernameOrEmail = dto.Email,
                AttemptedAt = NowPhilippines(),
                WasSuccessful = true,
                IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                TerminalName = Environment.MachineName
            });

            // 4. Start the user session using the real user.Id
            await _authLogService.StartUserSessionAsync(new UserSessionDto
            {
                UserId = userEntity.Id,
                LoginTime = NowPhilippines(),
                IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                TerminalName = Environment.MachineName
            });

            // 5. Return the login response (token, etc.)
            return Ok(loginResult);
        }


        /// <summary>
        /// Logs out the current user (ends session)
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                // End the user session
                await _authLogService.EndUserSessionAsync(userId);

                // Log the logout as a system action
                await _authLogService.LogSystemActionAsync(new SystemLogDto
                {
                    UserId = userId,
                    Module = "Auth",
                    ActionType = "LOGOUT",
                    Description = $"User {userId} logged out",
                    Timestamp = NowPhilippines(),
                    IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                });
            }

            await HttpContext.SignOutAsync(); // or _userService.LogoutAsync()

            return NoContent();
        }

        // ---------------------
        // User Management
        // ---------------------

        /// <summary>
        /// List all active users
        /// </summary>
        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        /// <summary>
        /// Get a single user by ID
        /// </summary>
        [HttpGet("users/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found or inactive." });

            return Ok(user);
        }

        /// <summary>
        /// Create a new user (Admin only)
        /// </summary>
        [HttpPost("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _userService.CreateUserAsync(dto);
            if (!success)
                return BadRequest(new { message = "User creation failed. Email may already be in use." });

            return Ok(new { message = "User created successfully." });
        }

        /// <summary>
        /// Update user details (FirstName, LastName, Email, Role)
        /// </summary>
        [HttpPut("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _userService.UpdateAsync(dto);
            if (!success)
                return NotFound(new { message = "User not found or update failed." });

            return Ok(new { message = "User updated successfully." });
        }

        /// <summary>
        /// Deactivate a user account (soft delete)
        /// </summary>
        [HttpPut("users/{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateUser(string id)
        {
            var success = await _userService.DeactivateAsync(id);
            if (!success)
                return NotFound(new { message = "User not found or already deactivated." });

            return Ok(new { message = "User deactivated successfully." });
        }
    }
}
