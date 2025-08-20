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
    public class SaleService : ISaleService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<SaleService> _logger;

        public SaleService(ApplicationDbContext context, IMapper mapper, ILogger<SaleService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SaleReadDto>> GetAllAsync(DateTime? from = null, DateTime? to = null, string? cashierUserId = null, string? invoiceNumber = null)
        {
            var q = _context.Sales
                .Include(s => s.Cashier)
                .Include(s => s.SaleItems).ThenInclude(i => i.Product)
                .Include(s => s.Payments)
                .Include(s => s.Discounts)
                .AsNoTracking()
                .AsQueryable();

            if (from.HasValue) q = q.Where(s => s.SaleDate >= from.Value);
            if (to.HasValue) q = q.Where(s => s.SaleDate <= to.Value);
            if (!string.IsNullOrWhiteSpace(cashierUserId)) q = q.Where(s => s.CashierId == cashierUserId);
            if (!string.IsNullOrWhiteSpace(invoiceNumber)) q = q.Where(s => s.InvoiceNumber == invoiceNumber);

            var list = await q.OrderByDescending(s => s.SaleDate).ToListAsync();
            return _mapper.Map<IEnumerable<SaleReadDto>>(list);
        }

        public async Task<SaleReadDto?> GetByIdAsync(int id)
        {
            var s = await _context.Sales
                .Include(sale => sale.Cashier)
                .Include(sale => sale.SaleItems).ThenInclude(i => i.Product)
                .Include(sale => sale.Payments)
                .Include(sale => sale.Discounts)
                .Include(sale => sale.Returns)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return s == null ? null : _mapper.Map<SaleReadDto>(s);
        }

        public async Task<SaleReadDto?> GetByInvoiceNumberAsync(string invoiceNumber)
        {
            var s = await _context.Sales
                .Include(sale => sale.Cashier)
                .Include(sale => sale.SaleItems).ThenInclude(i => i.Product)
                .Include(sale => sale.Payments)
                .Include(sale => sale.Discounts)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.InvoiceNumber == invoiceNumber);

            return s == null ? null : _mapper.Map<SaleReadDto>(s);
        }

        public async Task<SaleReadDto> CreateSaleAsync(SaleCreateDto dto, string cashierUserId)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Items == null || !dto.Items.Any()) throw new InvalidOperationException("Sale must contain at least one item.");

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var sale = _mapper.Map<Sale>(dto);
                sale.CashierId = cashierUserId;
                sale.SaleDate = dto.SaleDate ?? DateTime.UtcNow;

                // Compute item totals and sale totals
                decimal subTotal = 0m;
                decimal totalDiscount = 0m;

                // Ensure each product exists and set CostPrice if needed
                foreach (var si in sale.SaleItems)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == si.ProductId);
                    if (product == null)
                        throw new InvalidOperationException($"Product id {si.ProductId} not found.");

                    // compute discount amount and computed total
                    si.CalculateTotal();
                    subTotal += (si.UnitPrice * si.Quantity);
                    totalDiscount += si.DiscountAmount;

                    //// Optional: set cost price snapshot if SaleItem has CostPrice property filled earlier by mapper
                    //if (si.CostPrice <= 0 && product.CostPrice.HasValue)
                    //{
                    //    si.CostPrice = product.CostPrice.Value;
                    //}
                }

                sale.SubTotal = Math.Round(subTotal, 2);
                sale.TotalDiscount = Math.Round(totalDiscount, 2);
                sale.TotalAmount = Math.Round(sale.SubTotal - sale.TotalDiscount, 2);

                // Save sale
                _context.Sales.Add(sale);
                await _context.SaveChangesAsync();

                // Create inventory transactions (sale => decrease stock)
                foreach (var si in sale.SaleItems)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == si.ProductId);
                    if (product != null)
                    {
                        // Deduct on-hand
                        if (product.OnHand < si.Quantity)
                        {
                            // Option: allow negative on-hand or throw. We throw for now.
                            throw new InvalidOperationException($"Insufficient stock for product '{product.Name}'.");
                        }

                        product.OnHand -= si.Quantity;
                        _context.Products.Update(product);
                    }

                    var invTx = new InventoryTransaction
                    {
                        ProductId = si.ProductId,
                        ActionType = InventoryActionType.Sale,
                        Quantity = -Math.Abs(si.Quantity),
                        UnitCost = si.CostPrice,
                        ReferenceNumber = $"SALE-{sale.Id}",
                        Remarks = $"Sale {sale.InvoiceNumber}",
                        TransactionDate = DateTime.UtcNow,
                        PerformedById = cashierUserId,
                        ReferenceId = sale.Id,
                        ReferenceType = "Sale"
                    };

                    _context.InventoryTransactions.Add(invTx);
                }

                await _context.SaveChangesAsync();

                await tx.CommitAsync();

                var created = await _context.Sales
                    .Include(s => s.Cashier)
                    .Include(s => s.SaleItems).ThenInclude(i => i.Product)
                    .Include(s => s.Payments)
                    .Include(s => s.Discounts)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == sale.Id);

                return _mapper.Map<SaleReadDto>(created!);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Failed to create sale");
                throw;
            }
        }

        public async Task<VoidTransactionReadDto> VoidSaleAsync(int saleId, string voidedByUserId, string reason, bool isSystemVoid = false, string? approvalUserId = null)
        {
            var sale = await _context.Sales
                .Include(s => s.SaleItems)
                .FirstOrDefaultAsync(s => s.Id == saleId);

            if (sale == null) throw new KeyNotFoundException("Sale not found");

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // Create a void transaction record
                var vt = new VoidTransaction
                {
                    SaleId = saleId,
                    VoidedByUserId = voidedByUserId,
                    VoidedAt = DateTime.UtcNow,
                    Reason = reason,
                    TerminalIdentifier = null,
                    OriginalCashierUserId = sale.CashierId,
                    ApprovalUserId = approvalUserId,
                    IsSystemVoid = isSystemVoid
                };

                _context.VoidTransactions.Add(vt);

                // Restore inventory (StockIn) for each sale item
                foreach (var si in sale.SaleItems ?? Enumerable.Empty<SaleItem>())
                {
                    // Create inventory in transaction
                    var invTx = new InventoryTransaction
                    {
                        ProductId = si.ProductId,
                        ActionType = InventoryActionType.StockIn,
                        Quantity = Math.Abs(si.Quantity),
                        UnitCost = si.CostPrice,
                        ReferenceNumber = $"VOID-{sale.Id}",
                        Remarks = $"Void sale {sale.InvoiceNumber}.",
                        TransactionDate = DateTime.UtcNow,
                        PerformedById = voidedByUserId,
                        ReferenceId = vt.Id,
                        ReferenceType = "VoidTransaction"
                    };

                    _context.InventoryTransactions.Add(invTx);

                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == si.ProductId);
                    if (product != null)
                    {
                        product.OnHand += si.Quantity;
                        _context.Products.Update(product);
                    }
                }

                // mark sale voided
                sale.Status = SaleStatus.Voided;
                sale.VoidedAt = DateTime.UtcNow;
                sale.VoidedByUserId = voidedByUserId;
                _context.Sales.Update(sale);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                // return the created VoidTransaction as DTO
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
                _logger.LogError(ex, "Failed to void sale {SaleId}", saleId);
                throw;
            }
        }

        public async Task<PaymentReadDto> AddPaymentAsync(int saleId, PaymentCreateDto dto, string performedByUserId)
        {
            var sale = await _context.Sales.FirstOrDefaultAsync(s => s.Id == saleId);
            if (sale == null) throw new KeyNotFoundException("Sale not found");

            var payment = _mapper.Map<Models.Sales.Payment>(dto);
            payment.SaleId = saleId;
            payment.UserId = performedByUserId ?? dto.UserId;

            // Default status if not set
            // payment.Status = PaymentStatus.Completed; // mapper or caller can set

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var created = await _context.Payments
                .Include(p => p.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == payment.Id);

            return _mapper.Map<PaymentReadDto>(created!);
        }

        public async Task<SaleReadDto> FullRefundAsync(int saleId, string performedByUserId, RefundMethod refundMethod)
        {
            var sale = await _context.Sales
                .Include(s => s.SaleItems)
                .FirstOrDefaultAsync(s => s.Id == saleId);

            if (sale == null) throw new KeyNotFoundException("Sale not found");
            if (sale.IsFullyRefunded) throw new InvalidOperationException("Sale already refunded.");

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // create stock-in transactions to restore inventory
                foreach (var si in sale.SaleItems ?? Enumerable.Empty<SaleItem>())
                {
                    var invTx = new InventoryTransaction
                    {
                        ProductId = si.ProductId,
                        ActionType = InventoryActionType.StockIn,
                        Quantity = Math.Abs(si.Quantity),
                        UnitCost = si.CostPrice,
                        ReferenceNumber = $"REFUND-{sale.Id}",
                        Remarks = $"Full refund of sale {sale.InvoiceNumber}",
                        TransactionDate = DateTime.UtcNow,
                        PerformedById = performedByUserId,
                        ReferenceId = sale.Id,
                        ReferenceType = "FullRefund"
                    };

                    _context.InventoryTransactions.Add(invTx);

                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == si.ProductId);
                    if (product != null)
                    {
                        product.OnHand += si.Quantity;
                        _context.Products.Update(product);
                    }
                }

                // mark sale refunded
                sale.IsFullyRefunded = true;
                sale.RefundedAt = DateTime.UtcNow;
                sale.Status = SaleStatus.Refunded; // ensure you have Refunded in SaleStatus enum
                _context.Sales.Update(sale);

                // TODO: create Payment(s) record(s) to reflect the refund in your accounting. For now, you may create a Payment with Status=Refunded if you wish.

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                var updated = await _context.Sales
                    .Include(s => s.Cashier)
                    .Include(s => s.SaleItems).ThenInclude(i => i.Product)
                    .Include(s => s.Payments)
                    .Include(s => s.Discounts)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == sale.Id);

                // Create a SaleAuditTrail entry (optional)
                var audit = new SaleAuditTrail
                {
                    SaleId = sale.Id,
                    ActionType = SaleAuditActionType.FullRefund,
                    ActionAt = DateTime.UtcNow,
                    PerformedByUserId = performedByUserId,
                    Details = $"Full refund processed for sale {sale.InvoiceNumber} (method {refundMethod})."
                };
                _context.SaleAuditTrails.Add(audit);
                await _context.SaveChangesAsync();

                return _mapper.Map<SaleReadDto>(updated!);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Failed to process full refund for sale {SaleId}", saleId);
                throw;
            }
        }

        public async Task<SaleReadDto?> UpdateSaleAsync(int saleId, SaleCreateDto dto)
        {
            var existing = await _context.Sales
                .Include(s => s.SaleItems)
                .FirstOrDefaultAsync(s => s.Id == saleId);

            if (existing == null) return null;

            // NOTE: updating sale items and totals is complex (inventory reconciliation). Keep simple:
            // We only update metadata (counter, remarks, senior citizen flag) here.
            if (dto.CounterId.HasValue) existing.CounterId = dto.CounterId;
            existing.IsSeniorCitizen = dto.IsSeniorCitizen;
            existing.Remarks = dto.Remarks;

            _context.Sales.Update(existing);
            await _context.SaveChangesAsync();

            var updated = await _context.Sales
                .Include(s => s.Cashier)
                .Include(s => s.SaleItems).ThenInclude(i => i.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == saleId);

            return _mapper.Map<SaleReadDto>(updated!);
        }

        public async Task<IEnumerable<SaleAuditTrailReadDto>> GetAuditTrailsAsync(int saleId)
        {
            var list = await _context.SaleAuditTrails
                .Include(a => a.PerformedBy)
                .Where(a => a.SaleId == saleId)
                .OrderByDescending(a => a.ActionAt)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<SaleAuditTrailReadDto>>(list);
        }
    }
}
