using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Sales;
using PointOfSaleSystem.Interfaces.Sales;

namespace PointOfSaleSystem.Services.Sales
{
    public class ReturnedItemService : IReturnedItemService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ReturnedItemService> _logger;

        public ReturnedItemService(ApplicationDbContext context, IMapper mapper, ILogger<ReturnedItemService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ReturnedItemReadDto>> GetAllAsync(int? returnTransactionId = null)
        {
            var q = _context.ReturnedItems
                .Include(ri => ri.Product)
                .AsNoTracking()
                .AsQueryable();

            if (returnTransactionId.HasValue)
                q = q.Where(x => x.ReturnTransactionId == returnTransactionId.Value);

            var list = await q.OrderByDescending(x => x.Id).ToListAsync();
            return _mapper.Map<IEnumerable<ReturnedItemReadDto>>(list);
        }

        public async Task<ReturnedItemReadDto?> GetByIdAsync(int id)
        {
            var entity = await _context.ReturnedItems
                .Include(ri => ri.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(ri => ri.Id == id);

            return entity == null ? null : _mapper.Map<ReturnedItemReadDto>(entity);
        }

        public async Task<ReturnedItemReadDto> CreateAsync(int returnTransactionId, ReturnedItemCreateDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            // Validate parent return transaction
            var rt = await _context.ReturnTransactions.FindAsync(returnTransactionId);
            if (rt == null) throw new KeyNotFoundException($"ReturnTransaction id {returnTransactionId} not found.");

            // Validate product
            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null) throw new KeyNotFoundException($"Product id {dto.ProductId} not found.");

            var entity = _mapper.Map<Models.Sales.ReturnedItem>(dto);
            entity.ReturnTransactionId = returnTransactionId;

            _context.ReturnedItems.Add(entity);
            await _context.SaveChangesAsync();

            // Recalculate parent total refund amount
            await RecalculateReturnTotalAsync(returnTransactionId);

            var created = await _context.ReturnedItems
                .Include(ri => ri.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(ri => ri.Id == entity.Id);

            return _mapper.Map<ReturnedItemReadDto>(created!);
        }

        public async Task<ReturnedItemReadDto?> UpdateAsync(int id, ReturnedItemUpdateDto dto)
        {
            var existing = await _context.ReturnedItems.FirstOrDefaultAsync(ri => ri.Id == id);
            if (existing == null) return null;

            // Validate product exists
            var product = await _context.Products.FindAsync(existing.ProductId);
            if (product == null) throw new KeyNotFoundException($"Product id {existing.ProductId} not found.");

            existing.Quantity = dto.Quantity;
            existing.UnitPrice = dto.UnitPrice;
            // If you added Remarks to the model, set it (DTO includes it)
            // existing.Remarks = dto.Remarks;

            _context.ReturnedItems.Update(existing);
            await _context.SaveChangesAsync();

            await RecalculateReturnTotalAsync(existing.ReturnTransactionId);

            var updated = await _context.ReturnedItems
                .Include(ri => ri.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(ri => ri.Id == existing.Id);

            return _mapper.Map<ReturnedItemReadDto>(updated!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.ReturnedItems.FirstOrDefaultAsync(ri => ri.Id == id);
            if (existing == null) return false;

            var returnTransactionId = existing.ReturnTransactionId;

            _context.ReturnedItems.Remove(existing);
            var ok = await _context.SaveChangesAsync() > 0;

            if (ok)
            {
                await RecalculateReturnTotalAsync(returnTransactionId);
            }

            return ok;
        }

        /// <summary>
        /// Recomputes the parent ReturnTransaction.TotalRefundAmount based on current items.
        /// Saves the parent.
        /// </summary>
        private async Task RecalculateReturnTotalAsync(int returnTransactionId)
        {
            var total = await _context.ReturnedItems
                .Where(i => i.ReturnTransactionId == returnTransactionId)
                .SumAsync(i => (decimal?)i.UnitPrice * i.Quantity) ?? 0m;

            var rt = await _context.ReturnTransactions.FindAsync(returnTransactionId);
            if (rt != null)
            {
                rt.TotalRefundAmount = total;
                _context.ReturnTransactions.Update(rt);
                await _context.SaveChangesAsync();
            }
        }
    }
}
