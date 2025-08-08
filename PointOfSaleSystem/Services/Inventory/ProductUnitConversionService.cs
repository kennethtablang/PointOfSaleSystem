using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Inventory;
using PointOfSaleSystem.Interfaces.Inventory;
using PointOfSaleSystem.Models.Inventory;

namespace PointOfSaleSystem.Services.Inventory
{
    public class ProductUnitConversionService : IProductUnitConversionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductUnitConversionService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductUnitConversionReadDto>> GetAllAsync()
        {
            var list = await _context.ProductUnitConversions
                .Include(puc => puc.FromUnit)
                .Include(puc => puc.ToUnit)
                .Include(puc => puc.Product)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductUnitConversionReadDto>>(list);
        }

        public async Task<ProductUnitConversionReadDto?> GetByIdAsync(int id)
        {
            var entity = await _context.ProductUnitConversions
                .Include(puc => puc.FromUnit)
                .Include(puc => puc.ToUnit)
                .Include(puc => puc.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return null;

            return _mapper.Map<ProductUnitConversionReadDto>(entity);
        }

        public async Task<IEnumerable<ProductUnitConversionReadDto>> GetByProductIdAsync(int productId)
        {
            var list = await _context.ProductUnitConversions
                .Where(x => x.ProductId == productId)
                .Include(puc => puc.FromUnit)
                .Include(puc => puc.ToUnit)
                .Include(puc => puc.Product)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductUnitConversionReadDto>>(list);
        }

        public async Task<ProductUnitConversionReadDto> CreateAsync(ProductUnitConversionCreateDto dto)
        {
            // Referential checks
            var productExists = await _context.Products.AnyAsync(p => p.Id == dto.ProductId);
            if (!productExists) throw new InvalidOperationException($"Product with id {dto.ProductId} does not exist.");

            var fromUnitExists = await _context.Units.AnyAsync(u => u.Id == dto.FromUnitId);
            if (!fromUnitExists) throw new InvalidOperationException($"FromUnit with id {dto.FromUnitId} does not exist.");

            var toUnitExists = await _context.Units.AnyAsync(u => u.Id == dto.ToUnitId);
            if (!toUnitExists) throw new InvalidOperationException($"ToUnit with id {dto.ToUnitId} does not exist.");

            // Business rule: prevent duplicate conversion record
            var duplicate = await _context.ProductUnitConversions.AnyAsync(x =>
                x.ProductId == dto.ProductId &&
                x.FromUnitId == dto.FromUnitId &&
                x.ToUnitId == dto.ToUnitId);
            if (duplicate) throw new InvalidOperationException("A conversion for the specified Product/FromUnit/ToUnit already exists.");

            // Map and save
            var entity = _mapper.Map<ProductUnitConversion>(dto);
            _context.ProductUnitConversions.Add(entity);
            await _context.SaveChangesAsync();

            // Re-fetch with includes for read DTO (names)
            var created = await _context.ProductUnitConversions
                .Include(puc => puc.FromUnit)
                .Include(puc => puc.ToUnit)
                .Include(puc => puc.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == entity.Id);

            return _mapper.Map<ProductUnitConversionReadDto>(created!);
        }

        public async Task<ProductUnitConversionReadDto> UpdateAsync(ProductUnitConversionUpdateDto dto)
        {
            var entity = await _context.ProductUnitConversions
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (entity == null) throw new KeyNotFoundException($"ProductUnitConversion with id {dto.Id} not found.");

            // Referential checks (product and units)
            var productExists = await _context.Products.AnyAsync(p => p.Id == dto.ProductId);
            if (!productExists) throw new InvalidOperationException($"Product with id {dto.ProductId} does not exist.");

            var fromUnitExists = await _context.Units.AnyAsync(u => u.Id == dto.FromUnitId);
            if (!fromUnitExists) throw new InvalidOperationException($"FromUnit with id {dto.FromUnitId} does not exist.");

            var toUnitExists = await _context.Units.AnyAsync(u => u.Id == dto.ToUnitId);
            if (!toUnitExists) throw new InvalidOperationException($"ToUnit with id {dto.ToUnitId} does not exist.");

            // Prevent duplicate for other records
            var duplicate = await _context.ProductUnitConversions.AnyAsync(x =>
                x.Id != dto.Id &&
                x.ProductId == dto.ProductId &&
                x.FromUnitId == dto.FromUnitId &&
                x.ToUnitId == dto.ToUnitId);
            if (duplicate) throw new InvalidOperationException("Another conversion with the same Product/FromUnit/ToUnit already exists.");

            // Map the updatable fields onto entity
            _mapper.Map(dto, entity);

            // Save
            await _context.SaveChangesAsync();

            // Return updated read dto
            var updated = await _context.ProductUnitConversions
                .Include(puc => puc.FromUnit)
                .Include(puc => puc.ToUnit)
                .Include(puc => puc.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == entity.Id);

            return _mapper.Map<ProductUnitConversionReadDto>(updated!);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.ProductUnitConversions.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) throw new KeyNotFoundException($"ProductUnitConversion with id {id} not found.");

            _context.ProductUnitConversions.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.ProductUnitConversions.AnyAsync(x => x.Id == id);
        }
    }
}
