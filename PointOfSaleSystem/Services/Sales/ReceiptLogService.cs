using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Sales;
using PointOfSaleSystem.Interfaces.Sales;
using PointOfSaleSystem.Models.Sales;

namespace PointOfSaleSystem.Services.Sales
{
    public class ReceiptLogService : IReceiptLogService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ReceiptLogService> _logger;

        public ReceiptLogService(ApplicationDbContext context, IMapper mapper, ILogger<ReceiptLogService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ReceiptLogReadDto>> GetAllAsync(int? saleId = null)
        {
            var q = _context.ReceiptLogs
                .Include(r => r.PrintedBy)
                .AsNoTracking()
                .AsQueryable();

            if (saleId.HasValue) q = q.Where(r => r.SaleId == saleId.Value);

            var list = await q.OrderByDescending(r => r.PrintedAt).ToListAsync();
            return _mapper.Map<IEnumerable<ReceiptLogReadDto>>(list);
        }

        public async Task<ReceiptLogReadDto?> GetByIdAsync(int id)
        {
            var r = await _context.ReceiptLogs
                .Include(x => x.PrintedBy)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return r == null ? null : _mapper.Map<ReceiptLogReadDto>(r);
        }

        public async Task<ReceiptLogReadDto> CreateAsync(ReceiptLogCreateDto dto, string printedByUserId)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            // Validate sale exists
            var sale = await _context.Sales.FindAsync(dto.SaleId);
            if (sale == null) throw new KeyNotFoundException($"Sale id {dto.SaleId} not found.");

            var entity = _mapper.Map<ReceiptLog>(dto);
            entity.PrintedByUserId = printedByUserId ?? dto.PrintedByUserId ?? string.Empty;
            entity.PrintedAt = dto.PrintedAt ?? DateTime.UtcNow;

            _context.ReceiptLogs.Add(entity);
            await _context.SaveChangesAsync();

            var created = await _context.ReceiptLogs
                .Include(x => x.PrintedBy)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == entity.Id);

            return _mapper.Map<ReceiptLogReadDto>(created!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.ReceiptLogs.FindAsync(id);
            if (existing == null) return false;

            _context.ReceiptLogs.Remove(existing);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<ReceiptLogReadDto>> GetBySaleIdAsync(int saleId)
        {
            var list = await _context.ReceiptLogs
                .Include(x => x.PrintedBy)
                .Where(r => r.SaleId == saleId)
                .AsNoTracking()
                .OrderByDescending(r => r.PrintedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ReceiptLogReadDto>>(list);
        }
    }
}
