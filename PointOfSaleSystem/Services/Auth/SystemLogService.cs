using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Auth;
using PointOfSaleSystem.Interfaces.Auth;
using PointOfSaleSystem.Models.Auth;

namespace PointOfSaleSystem.Services.Auth
{
    public class SystemLogService : ISystemLogService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SystemLogService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Returns all system logs (most recent first)
        public async Task<IEnumerable<SystemLogDto>> GetSystemLogsAsync()
        {
            var logs = await _context.SystemLogs
                .Include(l => l.User) // include ApplicationUser for PerformedBy mapping
                .OrderByDescending(l => l.Timestamp)
                .AsNoTracking()
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();

            return _mapper.Map<IEnumerable<SystemLogDto>>(logs);
        }

        public async Task<SystemLogDto?> GetSystemLogByIdAsync(int id)
        {
            var log = await _context.SystemLogs
                .Include(l => l.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id);

            return log == null ? null : _mapper.Map<SystemLogDto>(log);
        }

        // Create a system log
        public async Task LogSystemActionAsync(SystemLogDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var entity = new SystemLog
            {
                Timestamp = dto.Timestamp == default ? DateTime.UtcNow : dto.Timestamp,
                Module = dto.Module ?? string.Empty,
                ActionType = dto.ActionType ?? string.Empty,
                Description = dto.Description ?? string.Empty,
                DataBefore = dto.DataBefore,
                DataAfter = dto.DataAfter,
                IPAddress = dto.IPAddress,
                UserId = dto.UserId
            };

            _context.SystemLogs.Add(entity);
            await _context.SaveChangesAsync();
        }

    }
}
