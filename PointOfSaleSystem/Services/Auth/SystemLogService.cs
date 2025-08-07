using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Auth;
using PointOfSaleSystem.Interfaces.Auth;

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

        public async Task<IEnumerable<SystemLogDto>> GetAllAsync()
        {
            var logs = await _context.SystemLogs
                .Include(l => l.User)
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();

            return _mapper.Map<IEnumerable<SystemLogDto>>(logs);
        }

        public async Task<SystemLogDto?> GetByIdAsync(int id)
        {
            var log = await _context.SystemLogs
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id);

            return log == null
                ? null
                : _mapper.Map<SystemLogDto>(log);
        }

    }
}
