using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Settings;
using PointOfSaleSystem.Interfaces.Settings;
using PointOfSaleSystem.Models.Settings;

namespace PointOfSaleSystem.Services.Settings
{
    public class CounterService : ICounterService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CounterService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CounterReadDto> CreateAsync(CounterCreateDto dto)
        {
            var entity = _mapper.Map<Counter>(dto);
            entity.CreatedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;
            entity.IsActive = true;

            _context.Counters.Add(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<CounterReadDto>(entity);
        }

        public async Task<bool> SetActiveAsync(int id, bool isActive)
        {
            var entity = await _context.Counters.FindAsync(id);
            if (entity == null) return false;

            entity.IsActive = isActive;
            entity.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CounterReadDto>> GetAllAsync()
        {
            var counters = await _context.Counters
                .OrderBy(c => c.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CounterReadDto>>(counters);
        }

        public async Task<CounterReadDto?> GetByIdAsync(int id)
        {
            var counter = await _context.Counters.FindAsync(id);
            return counter == null
                ? null
                : _mapper.Map<CounterReadDto>(counter);
        }

        public async Task<bool> UpdateAsync(int id, CounterUpdateDto dto)
        {
            var entity = await _context.Counters.FindAsync(id);
            if (entity == null) return false;

            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
