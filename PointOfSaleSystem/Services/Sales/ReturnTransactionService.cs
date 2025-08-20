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
    public class ReturnTransactionService : IReturnTransactionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ReturnTransactionService> _logger;

        public ReturnTransactionService(ApplicationDbContext context, IMapper mapper, ILogger<ReturnTransactionService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ReturnTransactionReadDto>> GetAllAsync(int? saleId = null)
        {
            var q = _context.ReturnTransactions
                .Include(r => r.ReturnedBy)
                .Include(r => r.Items).ThenInclude(i => i.Product)
                .AsNoTracking()
                .AsQueryable();

            if (saleId.HasValue)
                q = q.Where(r => r.OriginalSaleId == saleId.Value);

            var list = await q.OrderByDescending(r => r.ReturnDate).ToListAsync();
            return _mapper.Map<IEnumerable<ReturnTransactionReadDto>>(list);
        }

        public async Task<ReturnTransactionReadDto?> GetByIdAsync(int id)
        {
            var rt = await _context.ReturnTransactions
                .Include(r => r.ReturnedBy)
                .Include(r => r.Items).ThenInclude(i => i.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            return rt == null ? null : _mapper.Map<ReturnTransactionReadDto>(rt);
        }

        public async Task<ReturnTransactionReadDto> CreateAsync(ReturnTransactionCreateDto dto, string returnedByUserId)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            // Validate sale exists
            var sale = await _context.Sales
                .Include(s => s.SaleItems)
                .FirstOrDefaultAsync(s => s.Id == dto.OriginalSaleId);

            if (sale == null) throw new KeyNotFoundException($"Original sale id {dto.OriginalSaleId} not found.");

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // map
                var entity = _mapper.Map<ReturnTransaction>(dto);
                entity.ReturnedByUserId = returnedByUserId ?? dto.ReturnedByUserId ?? string.Empty;
                entity.ReturnDate = dto.ReturnDate ?? DateTime.UtcNow;
                entity.Status = ReturnStatus.Pending; // default; can be changed by workflow

                _context.ReturnTransactions.Add(entity);
                await _context.SaveChangesAsync();

                // Map and add items
                // Items created via Automapper on initial map if configured; ensure Product exists & validate qty
                foreach (var item in entity.Items)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
                    if (product == null)
                        throw new InvalidOperationException($"Product id {item.ProductId} not found.");

                    // Optional: validate that returned quantity does not exceed originally sold quantity
                    var saleItem = sale.SaleItems?.FirstOrDefault(si => si.ProductId == item.ProductId);
                    if (saleItem != null)
                    {
                        var alreadyReturned = saleItem.ReturnedQuantity;
                        if (item.Quantity + alreadyReturned > saleItem.Quantity)
                        {
                            throw new InvalidOperationException($"Return quantity for product '{product.Name}' exceeds what was sold.");
                        }

                        // update returnedQuantity on sale item
                        saleItem.ReturnedQuantity += item.Quantity;
                        _context.SaleItems.Update(saleItem);
                    }
                }

                await _context.SaveChangesAsync();

                // Persist inventory transactions: customer returned items usually go back to stock (+)
                foreach (var item in entity.Items)
                {
                    var invTx = new InventoryTransaction
                    {
                        ProductId = item.ProductId,
                        ActionType = InventoryActionType.Return,
                        Quantity = Math.Abs(item.Quantity), // positive = stock in
                        UnitCost = null,
                        ReferenceNumber = $"RET-{entity.Id}",
                        Remarks = $"Return of {item.Quantity} units from sale {sale.InvoiceNumber}.",
                        TransactionDate = DateTime.UtcNow,
                        PerformedById = returnedByUserId,
                        ReferenceId = entity.Id,
                        ReferenceType = "ReturnTransaction"
                    };

                    _context.InventoryTransactions.Add(invTx);

                    // Update Product.OnHand
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
                    if (product != null)
                    {
                        product.OnHand += item.Quantity;
                        _context.Products.Update(product);
                    }
                }

                await _context.SaveChangesAsync();

                await tx.CommitAsync();

                var created = await _context.ReturnTransactions
                    .Include(r => r.ReturnedBy)
                    .Include(r => r.Items).ThenInclude(i => i.Product)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Id == entity.Id);

                return _mapper.Map<ReturnTransactionReadDto>(created!);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Failed to create return transaction for sale {SaleId}", dto.OriginalSaleId);
                throw;
            }
        }

        public async Task<ReturnTransactionReadDto?> UpdateAsync(int id, ReturnTransactionUpdateDto dto)
        {
            var existing = await _context.ReturnTransactions
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (existing == null) return null;

            // Only update metadata fields here; item-level changes should go through ReturnedItemService
            if (dto.ReturnDate.HasValue) existing.ReturnDate = dto.ReturnDate.Value;
            if (!string.IsNullOrWhiteSpace(dto.Reason)) existing.Reason = dto.Reason;
            if (!string.IsNullOrWhiteSpace(dto.TerminalIdentifier)) existing.TerminalIdentifier = dto.TerminalIdentifier;
            if (dto.RefundMethod.HasValue) existing.RefundMethod = dto.RefundMethod.Value;
            if (dto.Status.HasValue) existing.Status = dto.Status.Value;

            _context.ReturnTransactions.Update(existing);
            await _context.SaveChangesAsync();

            var updated = await _context.ReturnTransactions
                .Include(r => r.ReturnedBy)
                .Include(r => r.Items).ThenInclude(i => i.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            return _mapper.Map<ReturnTransactionReadDto>(updated!);
        }

        public async Task<ReturnTransactionReadDto?> ChangeStatusAsync(int id, ReturnStatus newStatus, string performedByUserId)
        {
            var existing = await _context.ReturnTransactions.FirstOrDefaultAsync(r => r.Id == id);
            if (existing == null) return null;

            existing.Status = newStatus;
            _context.ReturnTransactions.Update(existing);
            await _context.SaveChangesAsync();

            // Optionally create SaleAuditTrail record / notifications - TODO if needed
            var updated = await _context.ReturnTransactions
                .Include(r => r.ReturnedBy)
                .Include(r => r.Items).ThenInclude(i => i.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            return _mapper.Map<ReturnTransactionReadDto>(updated!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.ReturnTransactions
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (existing == null) return false;

            // Business decision: we will not automatically change Product.OnHand on deletion here.
            // If you want delete -> restore stock, add logic to deduct inventory transactions and decrease OnHand.

            _context.ReturnTransactions.Remove(existing);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
