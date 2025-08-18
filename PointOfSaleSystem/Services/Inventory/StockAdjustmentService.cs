using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Inventory;
using PointOfSaleSystem.Interfaces.Inventory;
using PointOfSaleSystem.Models.Inventory;

namespace PointOfSaleSystem.Services.Inventory
{
    public class StockAdjustmentService : IStockAdjustmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public StockAdjustmentService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StockAdjustmentListDto>> GetAllAsync()
        {
            var adjustments = await _context.StockAdjustments
                .Include(sa => sa.Product)
                .Include(sa => sa.Unit)
                .Include(sa => sa.AdjustedByUser)
                .ToListAsync();

            return _mapper.Map<IEnumerable<StockAdjustmentListDto>>(adjustments);
        }

        public async Task<StockAdjustmentReadDto?> GetByIdAsync(int id)
        {
            var adjustment = await _context.StockAdjustments
                .Include(sa => sa.Product)
                .Include(sa => sa.Unit)
                .Include(sa => sa.AdjustedByUser)
                .FirstOrDefaultAsync(sa => sa.Id == id);

            return adjustment == null ? null : _mapper.Map<StockAdjustmentReadDto>(adjustment);
        }

        public async Task<StockAdjustmentReadDto> CreateAsync(StockAdjustmentCreateDto dto, string userId)
        {
            var adjustment = _mapper.Map<StockAdjustment>(dto);
            adjustment.AdjustmentDate = DateTime.UtcNow;
            adjustment.AdjustedByUserId = userId;

            _context.StockAdjustments.Add(adjustment);
            await _context.SaveChangesAsync();

            // Map back to DTO with product/unit names
            var created = await _context.StockAdjustments
                .Include(sa => sa.Product)
                .Include(sa => sa.Unit)
                .Include(sa => sa.AdjustedByUser)
                .FirstAsync(sa => sa.Id == adjustment.Id);

            return _mapper.Map<StockAdjustmentReadDto>(created);
        }

        public async Task<StockAdjustmentReadDto?> UpdateAsync(int id, StockAdjustmentUpdateDto dto, string userId)
        {
            var adjustment = await _context.StockAdjustments.FirstOrDefaultAsync(sa => sa.Id == id);
            if (adjustment == null) return null;

            _mapper.Map(dto, adjustment);
            adjustment.AdjustmentDate = DateTime.UtcNow;
            adjustment.AdjustedByUserId = userId;

            _context.StockAdjustments.Update(adjustment);
            await _context.SaveChangesAsync();

            var updated = await _context.StockAdjustments
                .Include(sa => sa.Product)
                .Include(sa => sa.Unit)
                .Include(sa => sa.AdjustedByUser)
                .FirstAsync(sa => sa.Id == adjustment.Id);

            return _mapper.Map<StockAdjustmentReadDto>(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var adjustment = await _context.StockAdjustments.FirstOrDefaultAsync(sa => sa.Id == id);
            if (adjustment == null) return false;

            _context.StockAdjustments.Remove(adjustment);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
