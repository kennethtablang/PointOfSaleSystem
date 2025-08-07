using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Settings;
using PointOfSaleSystem.Interfaces.Settings;
using PointOfSaleSystem.Models.Settings;

namespace PointOfSaleSystem.Services.Settings
{
    public class BusinessProfileService : IBusinessProfileService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BusinessProfileService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BusinessProfileReadDto> CreateAsync(BusinessProfileCreateDto dto)
        {
            var entity = _mapper.Map<BusinessProfile>(dto);
            _context.BusinessProfiles.Add(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<BusinessProfileReadDto>(entity);
        }

        public async Task<BusinessProfileReadDto?> GetAsync()
        {
            var entity = await _context.BusinessProfiles.FirstOrDefaultAsync();
            return entity == null ? null : _mapper.Map<BusinessProfileReadDto>(entity);
        }

        public async Task<bool> UpdateAsync(BusinessProfileUpdateDto dto)
        {
            var entity = await _context.BusinessProfiles.FindAsync(dto.Id);
            if (entity == null) return false;

            _mapper.Map(dto, entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
