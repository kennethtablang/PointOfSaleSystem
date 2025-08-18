using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Auth;
using PointOfSaleSystem.Interfaces.Auth;
using PointOfSaleSystem.Models.Auth;

namespace PointOfSaleSystem.Services.Auth
{
    public class AuthLogService : IAuthLogService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AuthLogService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LoginAttemptLogDto>> GetLoginAttemptsAsync()
        {
            var attempts = await _context.LoginAttemptLogs
                .OrderByDescending(l => l.AttemptedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<LoginAttemptLogDto>>(attempts);
        }

        public async Task<IEnumerable<SystemLogDto>> GetSystemLogsAsync()
        {
            var logs = await _context.SystemLogs
                .Include(l => l.User)
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();

            return _mapper.Map<IEnumerable<SystemLogDto>>(logs);
        }

        public async Task<IEnumerable<UserSessionDto>> GetActiveUserSessionsAsync()
        {
            var sessions = await _context.UserSessions
                .Include(s => s.User)
                .Where(s => s.LogoutTime == null)
                .OrderByDescending(s => s.LoginTime)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserSessionDto>>(sessions);
        }


        public async Task EndUserSessionAsync(string userId)
        {
            var session = await _context.UserSessions
            .Where(s => s.UserId == userId && s.LogoutTime == null)
            .OrderByDescending(s => s.LoginTime)
            .FirstOrDefaultAsync();

            if (session != null)
            {
                session.LogoutTime = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task LogLoginAttemptAsync(LoginAttemptLogDto dto)
        {
            var log = _mapper.Map<LoginAttemptLog>(dto);
            await _context.LoginAttemptLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task LogSystemActionAsync(SystemLogDto dto)
        {
            var log = _mapper.Map<SystemLog>(dto);
            log.Timestamp = DateTime.UtcNow; // ensure server-side timestamp
            await _context.SystemLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task StartUserSessionAsync(UserSessionDto dto)
        {
            // Optionally end previous sessions
            var existingSession = await _context.UserSessions
                .Where(s => s.UserId == dto.UserId && s.LogoutTime == null)
                .FirstOrDefaultAsync();

            if (existingSession != null)
            {
                existingSession.LogoutTime = DateTime.Now;
            }

            var session = _mapper.Map<UserSession>(dto);
            session.LoginTime = DateTime.UtcNow;
            await _context.UserSessions.AddAsync(session);
            await _context.SaveChangesAsync();
        }
    }
}
