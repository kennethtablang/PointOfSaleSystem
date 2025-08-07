using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Settings;
using PointOfSaleSystem.Interfaces.Settings;
using PointOfSaleSystem.Models.Settings;

namespace PointOfSaleSystem.Services.Settings
{
    public class VatSettingService : IVatSettingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public VatSettingService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<VatSettingReadDto> CreateAsync(VatSettingCreateDto dto)
        {
            var entity = _mapper.Map<VatSetting>(dto);
            _context.VatSettings.Add(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<VatSettingReadDto>(entity);
        }

        public async Task<IEnumerable<VatSettingReadDto>> GetAllAsync()
        {
            var entities = await _context.VatSettings
                .OrderByDescending(v => v.Id)
                .ToListAsync();

            return _mapper.Map<IEnumerable<VatSettingReadDto>>(entities);
        }

        public async Task<VatSettingReadDto?> GetByIdAsync(int id)
        {
            var entity = await _context.VatSettings.FindAsync(id);
            return entity == null
                ? null
                : _mapper.Map<VatSettingReadDto>(entity);
        }

        public async Task<bool> SetActiveAsync(int id, bool isActive)
        {
            var entity = await _context.VatSettings.FindAsync(id);
            if (entity == null) return false;

            entity.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(VatSettingUpdateDto dto)
        {
            var entity = await _context.VatSettings.FindAsync(dto.Id);
            if (entity == null) return false;

            // Apply updates
            _mapper.Map(dto, entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
