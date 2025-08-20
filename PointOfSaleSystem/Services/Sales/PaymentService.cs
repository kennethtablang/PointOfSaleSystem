using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Sales;
using PointOfSaleSystem.Interfaces.Sales;
using PointOfSaleSystem.Models.Sales;

namespace PointOfSaleSystem.Services.Sales
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(ApplicationDbContext context, IMapper mapper, ILogger<PaymentService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<PaymentReadDto>> GetAllAsync(int? saleId = null)
        {
            var q = _context.Payments
                .Include(p => p.User)
                .AsNoTracking()
                .AsQueryable();

            if (saleId.HasValue) q = q.Where(p => p.SaleId == saleId.Value);

            var list = await q.OrderByDescending(p => p.PaymentDate).ToListAsync();
            return _mapper.Map<IEnumerable<PaymentReadDto>>(list);
        }

        public async Task<PaymentReadDto?> GetByIdAsync(int id)
        {
            var p = await _context.Payments
                .Include(x => x.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return p == null ? null : _mapper.Map<PaymentReadDto>(p);
        }

        public async Task<PaymentReadDto> CreateAsync(int saleId, PaymentCreateDto dto, string performedByUserId)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var sale = await _context.Sales.FindAsync(saleId);
            if (sale == null) throw new KeyNotFoundException($"Sale id {saleId} not found.");

            var entity = _mapper.Map<Payment>(dto);
            entity.SaleId = saleId;
            entity.UserId = performedByUserId ?? dto.UserId ?? string.Empty;
            entity.PaymentDate = dto.PaymentDate ?? DateTime.UtcNow;

            // default status if not set by mapping
            // (mapper may set Status = Pending by default; keep it Completed unless business dictates otherwise)
            // if you want to process business logic (e.g., mark sale paid), do it here.

            _context.Payments.Add(entity);
            await _context.SaveChangesAsync();

            var created = await _context.Payments
                .Include(x => x.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == entity.Id);

            return _mapper.Map<PaymentReadDto>(created!);
        }

        public async Task<PaymentReadDto?> UpdateAsync(int id, PaymentCreateDto dto)
        {
            var existing = await _context.Payments.FirstOrDefaultAsync(p => p.Id == id);
            if (existing == null) return null;

            // allow updating amount, reference, change amount, terminal and date
            existing.Method = dto.Method;
            existing.Amount = dto.Amount;
            existing.ReferenceNumber = dto.ReferenceNumber;
            existing.PaymentDate = dto.PaymentDate ?? existing.PaymentDate;
            existing.ChangeAmount = dto.ChangeAmount;
            existing.Terminal = dto.Terminal;
            // do not change UserId unless authorizing system requires it
            if (!string.IsNullOrWhiteSpace(dto.UserId))
                existing.UserId = dto.UserId;

            _context.Payments.Update(existing);
            await _context.SaveChangesAsync();

            var updated = await _context.Payments
                .Include(x => x.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == existing.Id);

            return _mapper.Map<PaymentReadDto>(updated!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Payments.FindAsync(id);
            if (existing == null) return false;

            _context.Payments.Remove(existing);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<PaymentReadDto>> GetBySaleIdAsync(int saleId)
        {
            var list = await _context.Payments
                .Include(x => x.User)
                .Where(p => p.SaleId == saleId)
                .AsNoTracking()
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PaymentReadDto>>(list);
        }
    }
}
