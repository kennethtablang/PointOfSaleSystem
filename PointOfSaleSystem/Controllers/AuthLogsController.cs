using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSystem.DTOs.Auth;
using PointOfSaleSystem.Interfaces.Auth;

namespace PointOfSaleSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager")]
    public class AuthLogsController : ControllerBase
    {
        private readonly IAuthLogService _authLog;

        public AuthLogsController(IAuthLogService authLog)
        {
            _authLog = authLog;
        }

        // GET all login attempts
        [HttpGet("login-attempts")]
        public async Task<IActionResult> GetLoginAttempts()
        {
            var attempts = await _authLog.GetLoginAttemptsAsync();
            return Ok(attempts);
        }

        // GET all system logs
        [HttpGet("system-logs")]
        public async Task<IActionResult> GetSystemLogs()
        {
            var logs = await _authLog.GetSystemLogsAsync();
            return Ok(logs);
        }

        // GET all active user sessions
        [HttpGet("user-sessions")]
        public async Task<IActionResult> GetActiveUserSessions()
        {
            var sessions = await _authLog.GetActiveUserSessionsAsync();
            return Ok(sessions);
        }

        // POST a login attempt (usually called internally in AccountController)
        [HttpPost("login-attempt")]
        [AllowAnonymous]
        public async Task<IActionResult> LogLoginAttempt([FromBody] LoginAttemptLogDto dto)
        {
            await _authLog.LogLoginAttemptAsync(dto);
            return NoContent();
        }

        // POST a system action (called wherever you need auditing)
        [HttpPost("system-action")]
        public async Task<IActionResult> LogSystemAction([FromBody] SystemLogDto dto)
        {
            await _authLog.LogSystemActionAsync(dto);
            return NoContent();
        }

        // POST start session (called in AccountController on successful login)
        [HttpPost("session/start")]
        public async Task<IActionResult> StartSession([FromBody] UserSessionDto dto)
        {
            await _authLog.StartUserSessionAsync(dto);
            return NoContent();
        }

        // POST end session (called on logout)
        [HttpPost("session/end/{userId}")]
        public async Task<IActionResult> EndSession(string userId)
        {
            await _authLog.EndUserSessionAsync(userId);
            return NoContent();
        }
    }
}
