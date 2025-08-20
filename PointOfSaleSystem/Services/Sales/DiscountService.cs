using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Sales;
using PointOfSaleSystem.Interfaces.Sales;
using PointOfSaleSystem.Models.Sales;

namespace PointOfSaleSystem.Services.Sales
{
    public class DiscountService : IDiscountService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<DiscountService> _logger;

        public DiscountService(ApplicationDbContext context, IMapper mapper, ILogger<DiscountService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<DiscountReadDto>> GetAllAsync(int? saleId = null, int? saleItemId = null)
        {
            var q = _context.Discounts
                .Include(d => d.AppliedByUser)
                .Include(d => d.ApprovedByUser)
                .Include(d => d.DiscountSetting)
                .AsNoTracking()
                .AsQueryable();

            if (saleId.HasValue) q = q.Where(d => d.SaleId == saleId.Value);
            if (saleItemId.HasValue) q = q.Where(d => d.SaleItemId == saleItemId.Value);

            var list = await q.OrderByDescending(d => d.AppliedAt).ToListAsync();
            return _mapper.Map<IEnumerable<DiscountReadDto>>(list);
        }

        public async Task<DiscountReadDto?> GetByIdAsync(int id)
        {
            var d = await _context.Discounts
                .Include(x => x.AppliedByUser)
                .Include(x => x.ApprovedByUser)
                .Include(x => x.DiscountSetting)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return d == null ? null : _mapper.Map<DiscountReadDto>(d);
        }

        public async Task<DiscountReadDto> CreateAsync(DiscountCreateDto dto, string appliedByUserId)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            // validate sale
            var sale = await _context.Sales.FindAsync(dto.SaleId);
            if (sale == null) throw new KeyNotFoundException($"Sale id {dto.SaleId} not found.");

            // validate discount setting
            var ds = await _context.DiscountSettings.FindAsync(dto.DiscountSettingId);
            if (ds == null) throw new KeyNotFoundException($"DiscountSetting id {dto.DiscountSettingId} not found.");

            // build entity
            var entity = _mapper.Map<Discount>(dto);
            entity.AppliedByUserId = appliedByUserId ?? dto.AppliedByUserId ?? string.Empty;
            entity.AppliedAt = dto.AppliedAt ?? DateTime.UtcNow;

            // snapshot percent if not provided
            entity.PercentApplied = dto.PercentApplied ?? ds.DiscountPercent;

            _context.Discounts.Add(entity);

            // update sale total discount (simple incremental approach)
            sale.TotalDiscount += entity.DiscountAmount;
            _context.Sales.Update(sale);

            await _context.SaveChangesAsync();

            var created = await _context.Discounts
                .Include(x => x.AppliedByUser)
                .Include(x => x.ApprovedByUser)
                .Include(x => x.DiscountSetting)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == entity.Id);

            return _mapper.Map<DiscountReadDto>(created!);
        }

        public async Task<DiscountReadDto?> UpdateAsync(int id, DiscountUpdateDto dto)
        {
            var existing = await _context.Discounts.FirstOrDefaultAsync(d => d.Id == id);
            if (existing == null) return null;

            // track delta to update sale totals
            var oldAmount = existing.DiscountAmount;

            // apply updates
            existing.DiscountAmount = dto.DiscountAmount;
            if (dto.PercentApplied.HasValue) existing.PercentApplied = dto.PercentApplied.Value;
            if (!string.IsNullOrWhiteSpace(dto.Reason)) existing.Reason = dto.Reason;
            if (!string.IsNullOrWhiteSpace(dto.ApprovedByUserId)) existing.ApprovedByUserId = dto.ApprovedByUserId;
            if (dto.AppliedAt.HasValue) existing.AppliedAt = dto.AppliedAt.Value;

            _context.Discounts.Update(existing);

            // update sale totals
            var sale = await _context.Sales.FindAsync(existing.SaleId);
            if (sale != null)
            {
                sale.TotalDiscount = sale.TotalDiscount - oldAmount + existing.DiscountAmount;
                _context.Sales.Update(sale);
            }

            await _context.SaveChangesAsync();

            var updated = await _context.Discounts
                .Include(x => x.AppliedByUser)
                .Include(x => x.ApprovedByUser)
                .Include(x => x.DiscountSetting)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == existing.Id);

            return _mapper.Map<DiscountReadDto>(updated!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Discounts.FindAsync(id);
            if (existing == null) return false;

            // adjust sale totals
            var sale = await _context.Sales.FindAsync(existing.SaleId);
            if (sale != null)
            {
                sale.TotalDiscount -= existing.DiscountAmount;
                if (sale.TotalDiscount < 0) sale.TotalDiscount = 0;
                _context.Sales.Update(sale);
            }

            _context.Discounts.Remove(existing);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<DiscountReadDto>> GetBySaleIdAsync(int saleId)
        {
            var list = await _context.Discounts
                .Include(x => x.AppliedByUser)
                .Include(x => x.ApprovedByUser)
                .Include(x => x.DiscountSetting)
                .Where(d => d.SaleId == saleId)
                .AsNoTracking()
                .OrderByDescending(d => d.AppliedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DiscountReadDto>>(list);
        }
    }
}
