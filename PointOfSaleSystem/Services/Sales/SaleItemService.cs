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
    public class SaleItemService : ISaleItemService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<SaleItemService> _logger;

        public SaleItemService(ApplicationDbContext context, IMapper mapper, ILogger<SaleItemService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SaleItemReadDto>> GetBySaleIdAsync(int saleId)
        {
            var list = await _context.SaleItems
                .Include(si => si.Product)
                .Include(si => si.Unit)
                .Where(si => si.SaleId == saleId)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<SaleItemReadDto>>(list);
        }

        public async Task<SaleItemReadDto?> GetByIdAsync(int id)
        {
            var item = await _context.SaleItems
                .Include(si => si.Product)
                .Include(si => si.Unit)
                .AsNoTracking()
                .FirstOrDefaultAsync(si => si.Id == id);

            return item == null ? null : _mapper.Map<SaleItemReadDto>(item);
        }

        public async Task<SaleItemReadDto> CreateAsync(int saleId, SaleItemCreateDto dto, string performedByUserId)
        {
            // Validate sale
            var sale = await _context.Sales
                .Include(s => s.SaleItems)
                .FirstOrDefaultAsync(s => s.Id == saleId);

            if (sale == null) throw new KeyNotFoundException("Sale not found.");
            if (sale.Status == SaleStatus.Voided || sale.Status == SaleStatus.Refunded)
                throw new InvalidOperationException("Cannot add item to voided or refunded sale.");

            // Validate product
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == dto.ProductId);
            if (product == null) throw new KeyNotFoundException("Product not found.");

            if (product.OnHand < dto.Quantity)
                throw new InvalidOperationException($"Insufficient stock for product '{product.Name}'.");

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var entity = _mapper.Map<SaleItem>(dto);
                entity.SaleId = saleId;
                entity.CostPrice = dto.CostPrice ?? 0m;
                entity.CalculateTotal();

                _context.SaleItems.Add(entity);

                // Deduct inventory and create inventory transaction
                product.OnHand -= entity.Quantity;
                _context.Products.Update(product);

                var invTx = new InventoryTransaction
                {
                    ProductId = entity.ProductId,
                    ActionType = InventoryActionType.Sale,
                    Quantity = -Math.Abs(entity.Quantity),
                    UnitCost = entity.CostPrice,
                    ReferenceNumber = $"SALE-{saleId}",
                    Remarks = $"Added item to sale {sale.InvoiceNumber}",
                    TransactionDate = DateTime.UtcNow,
                    PerformedById = performedByUserId,
                    ReferenceId = saleId,
                    ReferenceType = "SaleItemCreate"
                };
                _context.InventoryTransactions.Add(invTx);

                // Update sale totals
                sale.SubTotal += (entity.UnitPrice * entity.Quantity);
                sale.TotalDiscount += entity.DiscountAmount;
                sale.TotalAmount = sale.SubTotal - sale.TotalDiscount;
                _context.Sales.Update(sale);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                var created = await _context.SaleItems
                    .Include(si => si.Product)
                    .Include(si => si.Unit)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(si => si.Id == entity.Id);

                return _mapper.Map<SaleItemReadDto>(created!);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Failed to add sale item");
                throw;
            }
        }

        public async Task<SaleItemReadDto?> UpdateAsync(int id, SaleItemCreateDto dto, string performedByUserId)
        {
            var existing = await _context.SaleItems
                .Include(si => si.Product)
                .Include(si => si.Sale)
                .FirstOrDefaultAsync(si => si.Id == id);

            if (existing == null) return null;
            if (existing.Sale != null && (existing.Sale.Status == SaleStatus.Voided || existing.Sale.Status == SaleStatus.Refunded))
                throw new InvalidOperationException("Cannot modify items of voided/refunded sale.");

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // Handle inventory difference
                var product = existing.Product;
                var qtyDiff = dto.Quantity - existing.Quantity;

                if (qtyDiff > 0 && product.OnHand < qtyDiff)
                    throw new InvalidOperationException($"Insufficient stock to increase quantity for '{product.Name}'.");

                // update product onHand
                product.OnHand -= qtyDiff;
                _context.Products.Update(product);

                // update item fields
                existing.Quantity = dto.Quantity;
                existing.UnitId = dto.UnitId;
                existing.UnitPrice = dto.UnitPrice;
                existing.CostPrice = dto.CostPrice ?? existing.CostPrice;
                existing.DiscountPercent = dto.DiscountPercent;
                existing.CalculateTotal();

                _context.SaleItems.Update(existing);

                // create inventory transaction for change (if qtyDiff != 0)
                if (qtyDiff != 0)
                {
                    var invTx = new InventoryTransaction
                    {
                        ProductId = existing.ProductId,
                        ActionType = InventoryActionType.Adjustment,
                        Quantity = -qtyDiff, // if qty increased, negative to decrease stock
                        UnitCost = existing.CostPrice,
                        ReferenceNumber = $"SALEITEM-UPDATE-{existing.Id}",
                        Remarks = $"Updated sale item {existing.Id}",
                        TransactionDate = DateTime.UtcNow,
                        PerformedById = performedByUserId,
                        ReferenceId = existing.Id,
                        ReferenceType = "SaleItemUpdate"
                    };
                    _context.InventoryTransactions.Add(invTx);
                }

                // update sale totals
                var sale = existing.Sale!;
                // recompute sale aggregates simply by re-summing items (safer than incremental)
                var allItems = await _context.SaleItems.Where(si => si.SaleId == sale.Id).ToListAsync();
                sale.SubTotal = allItems.Sum(i => i.UnitPrice * i.Quantity);
                sale.TotalDiscount = allItems.Sum(i => i.DiscountAmount);
                sale.TotalAmount = sale.SubTotal - sale.TotalDiscount;
                _context.Sales.Update(sale);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                var updated = await _context.SaleItems
                    .Include(si => si.Product)
                    .Include(si => si.Unit)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(si => si.Id == id);

                return _mapper.Map<SaleItemReadDto>(updated!);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Failed to update sale item {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id, string performedByUserId)
        {
            var existing = await _context.SaleItems
                .Include(si => si.Product)
                .Include(si => si.Sale)
                .FirstOrDefaultAsync(si => si.Id == id);

            if (existing == null) return false;
            if (existing.Sale != null && (existing.Sale.Status == SaleStatus.Voided || existing.Sale.Status == SaleStatus.Refunded))
                throw new InvalidOperationException("Cannot delete items from voided/refunded sale.");

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // restore product onHand
                var product = existing.Product;
                product.OnHand += existing.Quantity;
                _context.Products.Update(product);

                // create inventory transaction to reverse sale
                var invTx = new InventoryTransaction
                {
                    ProductId = existing.ProductId,
                    ActionType = InventoryActionType.StockIn,
                    Quantity = Math.Abs(existing.Quantity),
                    UnitCost = existing.CostPrice,
                    ReferenceNumber = $"SALEITEM-DELETE-{existing.Id}",
                    Remarks = $"Deleted sale item {existing.Id}",
                    TransactionDate = DateTime.UtcNow,
                    PerformedById = performedByUserId,
                    ReferenceId = existing.Id,
                    ReferenceType = "SaleItemDelete"
                };
                _context.InventoryTransactions.Add(invTx);

                // update sale totals by recomputing remaining items
                var sale = existing.Sale!;
                _context.SaleItems.Remove(existing);

                await _context.SaveChangesAsync();

                var remainingItems = await _context.SaleItems.Where(si => si.SaleId == sale.Id).ToListAsync();
                sale.SubTotal = remainingItems.Sum(i => i.UnitPrice * i.Quantity);
                sale.TotalDiscount = remainingItems.Sum(i => i.DiscountAmount);
                sale.TotalAmount = sale.SubTotal - sale.TotalDiscount;
                _context.Sales.Update(sale);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Failed to delete sale item {Id}", id);
                throw;
            }
        }
    }
}
