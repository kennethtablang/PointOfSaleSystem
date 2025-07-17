using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Inventory.Category;
using PointOfSaleSystem.Interfaces.Inventory;
using PointOfSaleSystem.Models.Inventory;

namespace PointOfSaleSystem.Services.Inventory
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CategoryService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryViewDto>> GetAllAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            return _mapper.Map<IEnumerable<CategoryViewDto>>(categories);
        }

        public async Task<CategoryViewDto?> GetByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            return category == null ? null : _mapper.Map<CategoryViewDto>(category);
        }

        public async Task<CategoryViewDto> CreateAsync(CategoryCreateDto dto)
        {
            var category = _mapper.Map<Category>(dto); // ✅ Map from CreateDto → Entity
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return _mapper.Map<CategoryViewDto>(category); // ✅ Return ViewDto
        }

        public async Task<bool> UpdateAsync(CategoryUpdateDto dto)
        {
            var category = await _context.Categories.FindAsync(dto.Id);
            if (category == null) return false;

            _mapper.Map(dto, category); // ✅ Map UpdateDto → existing entity
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
