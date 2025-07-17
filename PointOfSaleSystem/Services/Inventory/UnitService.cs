using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Inventory.Unit;
using PointOfSaleSystem.Interfaces.Inventory;
using PointOfSaleSystem.Models.Inventory;

namespace PointOfSaleSystem.Services.Inventory
{
    public class UnitService : IUnitService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UnitService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UnitViewDto>> GetAllAsync()
        {
            var units = await _context.Units.ToListAsync();
            return _mapper.Map<IEnumerable<UnitViewDto>>(units);
        }

        public async Task<UnitViewDto?> GetByIdAsync(int id)
        {
            var unit = await _context.Units.FindAsync(id);
            return unit == null ? null : _mapper.Map<UnitViewDto>(unit);
        }

        public async Task<UnitViewDto> CreateAsync(UnitCreateDto dto)
        {
            var unit = _mapper.Map<Unit>(dto);
            _context.Units.Add(unit);
            await _context.SaveChangesAsync();
            return _mapper.Map<UnitViewDto>(unit);
        }

        public async Task<bool> UpdateAsync(UnitUpdateDto dto)
        {
            var unit = await _context.Units.FindAsync(dto.Id);
            if (unit == null) return false;

            _mapper.Map(dto, unit);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var unit = await _context.Units.FindAsync(id);
            if (unit == null) return false;

            _context.Units.Remove(unit);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
