using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Settings;
using PointOfSaleSystem.Interfaces.Settings;
using PointOfSaleSystem.Models.Settings;

namespace PointOfSaleSystem.Services.Settings
{
    public class ReceiptSettingService : IReceiptSettingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ReceiptSettingService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ReceiptSettingReadDto> CreateAsync(ReceiptSettingCreateDto dto)
        {
            var entity = _mapper.Map<ReceiptSetting>(dto);
            entity.IsActive = true;

            _context.ReceiptSettings.Add(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ReceiptSettingReadDto>(entity);
        }

        public async Task<IEnumerable<ReceiptSettingReadDto>> GetAllAsync()
        {
            var list = await _context.ReceiptSettings
                .AsNoTracking()
                .OrderBy(r => r.Id)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ReceiptSettingReadDto>>(list);
        }

        public async Task<ReceiptSettingReadDto?> GetByIdAsync(int id)
        {
            var entity = await _context.ReceiptSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            return entity == null
                ? null
                : _mapper.Map<ReceiptSettingReadDto>(entity);
        }

        public async Task<bool> SetActiveAsync(int id, bool isActive)
        {
            var entity = await _context.ReceiptSettings.FindAsync(id);
            if (entity == null) return false;

            entity.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(int id, ReceiptSettingUpdateDto dto)
        {
            var entity = await _context.ReceiptSettings.FindAsync(id);
            if (entity == null) return false;

            _mapper.Map(dto, entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
