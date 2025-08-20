using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Sales;
using PointOfSaleSystem.Enums;
using PointOfSaleSystem.Interfaces.Sales;
using PointOfSaleSystem.Models.Inventory;
using PointOfSaleSystem.Models.Sales;

namespace PointOfSaleSystem.Services.Sales
{
    public class VoidTransactionService : IVoidTransactionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<VoidTransactionService> _logger;

        public VoidTransactionService(ApplicationDbContext context, IMapper mapper, ILogger<VoidTransactionService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<VoidTransactionReadDto>> GetAllAsync(int? saleId = null)
        {
            var q = _context.VoidTransactions
                .Include(v => v.VoidedBy)
                .Include(v => v.ApprovalUser)
                .Include(v => v.OriginalCashier)
                .AsNoTracking()
                .AsQueryable();

            if (saleId.HasValue) q = q.Where(v => v.SaleId == saleId.Value);

            var list = await q.OrderByDescending(v => v.VoidedAt).ToListAsync();
            return _mapper.Map<IEnumerable<VoidTransactionReadDto>>(list);
        }

        public async Task<VoidTransactionReadDto?> GetByIdAsync(int id)
        {
            var v = await _context.VoidTransactions
                .Include(vt => vt.VoidedBy)
                .Include(vt => vt.ApprovalUser)
                .Include(vt => vt.OriginalCashier)
                .AsNoTracking()
                .FirstOrDefaultAsync(vt => vt.Id == id);

            return v == null ? null : _mapper.Map<VoidTransactionReadDto>(v);
        }

        public async Task<VoidTransactionReadDto> CreateAsync(VoidTransactionCreateDto dto, string performedByUserId)
        {
            // locate sale
            var sale = await _context.Sales
                .Include(s => s.SaleItems)
                .FirstOrDefaultAsync(s => s.Id == dto.SaleId);

            if (sale == null) throw new KeyNotFoundException("Sale not found.");
            if (sale.Status == SaleStatus.Voided) throw new InvalidOperationException("Sale already voided.");
            if (sale.IsFullyRefunded) throw new InvalidOperationException("Sale already refunded; cannot void.");

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var vt = _mapper.Map<VoidTransaction>(dto);
                vt.VoidedByUserId = performedByUserId ?? dto.VoidedByUserId;
                vt.VoidedAt = dto.VoidedAt ?? DateTime.UtcNow;

                _context.VoidTransactions.Add(vt);
                await _context.SaveChangesAsync();

                // Restore inventory: create StockIn txs and increment product OnHand
                foreach (var si in sale.SaleItems ?? Enumerable.Empty<SaleItem>())
                {
                    var inv = new InventoryTransaction
                    {
                        ProductId = si.ProductId,
                        ActionType = InventoryActionType.StockIn,
                        Quantity = Math.Abs(si.Quantity),
                        UnitCost = si.CostPrice,
                        ReferenceNumber = $"VOID-{sale.Id}",
                        Remarks = $"Void sale {sale.InvoiceNumber}",
                        TransactionDate = DateTime.UtcNow,
                        PerformedById = performedByUserId,
                        ReferenceId = vt.Id,
                        ReferenceType = "VoidTransaction"
                    };
                    _context.InventoryTransactions.Add(inv);

                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == si.ProductId);
                    if (product != null)
                    {
                        product.OnHand += si.Quantity;
                        _context.Products.Update(product);
                    }
                }

                // mark sale as voided
                sale.Status = SaleStatus.Voided;
                sale.VoidedAt = DateTime.UtcNow;
                sale.VoidedByUserId = performedByUserId;
                _context.Sales.Update(sale);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                var created = await _context.VoidTransactions
                    .Include(v => v.VoidedBy)
                    .Include(v => v.ApprovalUser)
                    .Include(v => v.OriginalCashier)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(v => v.Id == vt.Id);

                return _mapper.Map<VoidTransactionReadDto>(created!);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Failed to create void transaction for sale {SaleId}", dto.SaleId);
                throw;
            }
        }

        public async Task<VoidTransactionReadDto?> ApproveAsync(int voidTransactionId, string approvalUserId, bool approved, string? approvalNotes = null)
        {
            var vt = await _context.VoidTransactions
                .Include(v => v.VoidedBy)
                .Include(v => v.ApprovalUser)
                .Include(v => v.OriginalCashier)
                .FirstOrDefaultAsync(v => v.Id == voidTransactionId);

            if (vt == null) return null;

            // record approval
            vt.ApprovalUserId = approvalUserId;
            _context.VoidTransactions.Update(vt);
            await _context.SaveChangesAsync();

            // record audit trail
            var audit = new SaleAuditTrail
            {
                SaleId = vt.SaleId,
                ActionType = approved ? SaleAuditActionType.VoidApproved : SaleAuditActionType.VoidRejected,
                ActionAt = DateTime.UtcNow,
                PerformedByUserId = approvalUserId,
                Details = $"Void transaction {(approved ? "approved" : "rejected")}. Notes: {approvalNotes}"
            };

            _context.SaleAuditTrails.Add(audit);
            await _context.SaveChangesAsync();

            var updated = await _context.VoidTransactions
                .Include(v => v.VoidedBy)
                .Include(v => v.ApprovalUser)
                .Include(v => v.OriginalCashier)
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == voidTransactionId);

            return _mapper.Map<VoidTransactionReadDto>(updated!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var vt = await _context.VoidTransactions.FindAsync(id);
            if (vt == null) return false;

            _context.VoidTransactions.Remove(vt);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
