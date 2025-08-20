using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Sales;
using PointOfSaleSystem.Interfaces.Sales;
using PointOfSaleSystem.Models.Sales;

namespace PointOfSaleSystem.Services.Sales
{
    public class SaleAuditTrailService : ISaleAuditTrailService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<SaleAuditTrailService> _logger;

        public SaleAuditTrailService(ApplicationDbContext context, IMapper mapper, ILogger<SaleAuditTrailService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SaleAuditTrailReadDto>> GetBySaleIdAsync(int saleId)
        {
            var list = await _context.SaleAuditTrails
                .Include(a => a.PerformedBy)
                .Where(a => a.SaleId == saleId)
                .OrderByDescending(a => a.ActionAt)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<SaleAuditTrailReadDto>>(list);
        }

        public async Task<SaleAuditTrailReadDto> CreateAsync(SaleAuditTrailCreateDto dto, string performedByUserId)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            // Validate sale exists
            var saleExists = await _context.Sales.AnyAsync(s => s.Id == dto.SaleId);
            if (!saleExists) throw new KeyNotFoundException($"Sale id {dto.SaleId} not found.");

            var entity = _mapper.Map<SaleAuditTrail>(dto);
            entity.PerformedByUserId = performedByUserId ?? dto.PerformedByUserId;
            entity.ActionAt = dto.ActionAt ?? DateTime.UtcNow;

            _context.SaleAuditTrails.Add(entity);
            await _context.SaveChangesAsync();

            var created = await _context.SaleAuditTrails
                .Include(a => a.PerformedBy)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == entity.Id);

            return _mapper.Map<SaleAuditTrailReadDto>(created!);
        }

        public async Task<IEnumerable<SaleAuditTrailReadDto>> QueryAsync(int? saleId = null, DateTime? from = null, DateTime? to = null)
        {
            var q = _context.SaleAuditTrails
                .Include(a => a.PerformedBy)
                .AsNoTracking()
                .AsQueryable();

            if (saleId.HasValue) q = q.Where(a => a.SaleId == saleId.Value);
            if (from.HasValue) q = q.Where(a => a.ActionAt >= from.Value);
            if (to.HasValue) q = q.Where(a => a.ActionAt <= to.Value);

            var list = await q.OrderByDescending(a => a.ActionAt).ToListAsync();
            return _mapper.Map<IEnumerable<SaleAuditTrailReadDto>>(list);
        }
    }
}
