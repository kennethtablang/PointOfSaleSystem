using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Auth;
using PointOfSaleSystem.Interfaces.Auth;

namespace PointOfSaleSystem.Services.Auth
{
    public class UserSessionService : IUserSessionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserSessionService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserSessionDto>> GetAllAsync()
        {
            var sessions = await _context.UserSessions
                .Include(s => s.User)
                .OrderByDescending(s => s.LoginTime)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserSessionDto>>(sessions);
        }

        public async Task<IEnumerable<UserSessionDto>> GetActiveAsync()
        {
            var active = await _context.UserSessions
                .Include(s => s.User)
                .Where(s => s.LogoutTime == null)
                .OrderByDescending(s => s.LoginTime)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserSessionDto>>(active);
        }

        public async Task<IEnumerable<UserSessionDto>> GetByUserIdAsync(string userId)
        {
            var list = await _context.UserSessions
                .Include(s => s.User)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.LoginTime)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserSessionDto>>(list);
        }
    }
}
