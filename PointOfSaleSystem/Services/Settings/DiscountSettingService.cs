using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Settings;
using PointOfSaleSystem.Interfaces.Settings;
using PointOfSaleSystem.Models.Settings;

namespace PointOfSaleSystem.Services.Settings
{
    public class DiscountSettingService : IDiscountSettingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DiscountSettingService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<DiscountSettingReadDto> CreateAsync(DiscountSettingCreateDto dto)
        {
            var discount = _mapper.Map<DiscountSetting>(dto);
            discount.CreatedAt = DateTime.Now;
            discount.UpdatedAt = DateTime.Now;

            _context.DiscountSettings.Add(discount);
            await _context.SaveChangesAsync();

            return _mapper.Map<DiscountSettingReadDto>(discount);
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var discount = await _context.DiscountSettings.FindAsync(id);
            if (discount == null || !discount.IsActive) return false;

            discount.IsActive = false;
            discount.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<DiscountSettingReadDto>> GetAllAsync()
        {
            var discounts = await _context.DiscountSettings
                .OrderByDescending(d => d.UpdatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DiscountSettingReadDto>>(discounts);
        }

        public async Task<DiscountSettingReadDto?> GetByIdAsync(int id)
        {
            var discount = await _context.DiscountSettings.FindAsync(id);
            return discount == null ? null : _mapper.Map<DiscountSettingReadDto>(discount);
        }

        public async Task<bool> UpdateAsync(int id, DiscountSettingUpdateDto dto)
        {
            var discount = await _context.DiscountSettings.FindAsync(id);
            if (discount == null) return false;

            _mapper.Map(dto, discount);
            discount.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;

        }
    }
}
