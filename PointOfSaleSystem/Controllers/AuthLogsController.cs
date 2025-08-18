using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSystem.DTOs.Auth;
using PointOfSaleSystem.Interfaces.Auth;
using System.Security.Claims;

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
            if (dto == null) return BadRequest();

            // Ensure server-side timestamp and IP if client didn't provide
            dto.AttemptedAt = dto.AttemptedAt == default ? DateTime.UtcNow : dto.AttemptedAt;
            dto.IPAddress = dto.IPAddress ?? GetClientIp();

            await _authLog.LogLoginAttemptAsync(dto);
            return NoContent();
        }

        // POST a system action (called wherever you need auditing)
        [HttpPost("system-action")]
        public async Task<IActionResult> LogSystemAction([FromBody] SystemLogDto dto)
        {
            if (dto == null) return BadRequest();

            // Fill server-side timestamp if missing
            dto.Timestamp = dto.Timestamp == default ? DateTime.UtcNow : dto.Timestamp;

            // Fill IP if not provided
            dto.IPAddress = dto.IPAddress ?? GetClientIp();

            // Fill UserId from current authenticated user if available (prevents spoofing)
            if (string.IsNullOrWhiteSpace(dto.UserId))
            {
                var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrWhiteSpace(userId))
                    dto.UserId = userId;
            }

            await _authLog.LogSystemActionAsync(dto);
            return NoContent();
        }

        // POST start session (called in AccountController on successful login)
        [HttpPost("session/start")]
        public async Task<IActionResult> StartSession([FromBody] UserSessionDto dto)
        {
            if (dto == null) return BadRequest();

            // server-side defaults
            dto.LoginTime = dto.LoginTime == default ? DateTime.UtcNow : dto.LoginTime;
            dto.IPAddress = dto.IPAddress ?? GetClientIp();

            // If user id missing and there's an authenticated user, use it
            if (string.IsNullOrWhiteSpace(dto.UserId))
            {
                var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrWhiteSpace(userId))
                    dto.UserId = userId;
            }

            await _authLog.StartUserSessionAsync(dto);
            return NoContent();
        }

        // POST end session (called on logout)
        [HttpPost("session/end/{userId}")]
        public async Task<IActionResult> EndSession(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return BadRequest();

            await _authLog.EndUserSessionAsync(userId);
            return NoContent();
        }

        // ---- Helpers ----
        private string? GetClientIp()
        {
            // Try common headers first (if behind proxy), then fall back to connection remote IP.
            if (Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
            {
                var first = forwarded.ToString().Split(',').FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(first)) return first.Trim();
            }

            if (Request.Headers.TryGetValue("X-Real-IP", out var realIp))
            {
                var ip = realIp.ToString().Split(',').FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(ip)) return ip.Trim();
            }

            return HttpContext?.Connection?.RemoteIpAddress?.ToString();
        }
    }
}
