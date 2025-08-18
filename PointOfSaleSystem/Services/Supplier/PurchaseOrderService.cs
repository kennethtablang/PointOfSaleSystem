using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Suppliers;
using PointOfSaleSystem.Enums;
using PointOfSaleSystem.Interfaces.Supplier;
using PointOfSaleSystem.Models.Inventory;
using PointOfSaleSystem.Models.Suppliers;
using System.Security.Cryptography;

namespace PointOfSaleSystem.Services.Supplier
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<PurchaseOrderService> _logger;

        private const int MaxPoGenerationAttempts = 10;

        public PurchaseOrderService(ApplicationDbContext context, IMapper mapper, ILogger<PurchaseOrderService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        private static string GeneratePurchaseOrderNumber()
        {
            // Use UTC with milliseconds to reduce collision windows
            var stamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmssfff"); // e.g. 20250813-160412123
                                                                        // cryptographically strong random 4-digit number
            int rand = RandomNumberGenerator.GetInt32(1000, 10000); // 1000..9999
            return $"PO-{stamp}-{rand}";
        }

        private async Task<string> GetUniquePurchaseOrderNumberAsync()
        {
            for (int attempt = 1; attempt <= MaxPoGenerationAttempts; attempt++)
            {
                var candidate = GeneratePurchaseOrderNumber();

                // quick DB check
                var exists = await _context.PurchaseOrders
                    .AsNoTracking()
                    .AnyAsync(p => p.PurchaseOrderNumber == candidate);

                if (!exists)
                {
                    // candidate appears unique — return it
                    return candidate;
                }

                _logger.LogWarning("PO number collision on attempt {Attempt} for candidate {Candidate}. Retrying...", attempt, candidate);

                // small delay could be added to reduce thundering if desired:
                // await Task.Delay(10 * attempt);
            }

            // If exhausted attempts, fail loudly
            _logger.LogError("Exhausted {MaxAttempts} attempts to generate a unique PurchaseOrderNumber.", MaxPoGenerationAttempts);
            throw new InvalidOperationException("Could not generate unique PurchaseOrderNumber. Please try again.");
        }

        public async Task<IEnumerable<PurchaseOrderReadDto>> GetAllAsync()
        {
            var list = await _context.PurchaseOrders
                .Include(p => p.Supplier)
                .Include(p => p.Items).ThenInclude(i => i.Product)
                .Include(p => p.Items).ThenInclude(i => i.Unit)
                .Include(p => p.ReceivedStocks).ThenInclude(rs => rs.PurchaseOrderItem).ThenInclude(pi => pi.Product)
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PurchaseOrderReadDto>>(list);
        }

        public async Task<PurchaseOrderReadDto?> GetByIdAsync(int id)
        {
            var po = await _context.PurchaseOrders
                .Include(p => p.Supplier)
                .Include(p => p.Items).ThenInclude(i => i.Product)
                .Include(p => p.Items).ThenInclude(i => i.Unit)
                .Include(p => p.ReceivedStocks)
                    .ThenInclude(rs => rs.PurchaseOrderItem)
                        .ThenInclude(pi => pi.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            return po == null ? null : _mapper.Map<PurchaseOrderReadDto>(po);
        }

        // returns POs that have Received status and at least one ReceivedStock.Processed == false
        public async Task<IEnumerable<PurchaseOrderReadDto>> GetPendingReceivingsAsync()
        {
            var pending = await _context.PurchaseOrders
                .Include(p => p.Supplier)
                .Include(p => p.Items).ThenInclude(i => i.Product)
                .Include(p => p.Items).ThenInclude(i => i.Unit)
                .Include(p => p.ReceivedStocks.Where(rs => !rs.Processed))
                    .ThenInclude(rs => rs.PurchaseOrderItem).ThenInclude(pi => pi.Product)
                .AsNoTracking()
                .Where(p => p.Status == PurchaseOrderStatus.Received && p.ReceivedStocks.Any(rs => !rs.Processed))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PurchaseOrderReadDto>>(pending);
        }

        public async Task PostReceivedToInventoryAsync(int purchaseOrderId, string processedByUserId)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // load unprocessed received records for this PO including purchase order item & product
                var recs = await _context.ReceivedStocks
                    .Include(r => r.PurchaseOrderItem)
                        .ThenInclude(pi => pi.Product)
                    .Where(r => r.PurchaseOrderId == purchaseOrderId && !r.Processed)
                    .ToListAsync();

                if (recs == null || !recs.Any())
                    throw new InvalidOperationException("No unprocessed received stock found for this purchase order.");

                // Create StockReceive header
                var stockReceive = new StockReceive
                {
                    PurchaseOrderId = purchaseOrderId,
                    ReceivedDate = DateTime.UtcNow,
                    ReceivedByUserId = processedByUserId,
                    ReferenceNumber = $"PO-{purchaseOrderId}-POST-{DateTime.UtcNow:yyyyMMddHHmmss}",
                    Remarks = "Posted from purchase order receives"
                };
                _context.StockReceives.Add(stockReceive);

                // We'll create items and inventory transactions
                foreach (var rec in recs)
                {
                    var poItem = rec.PurchaseOrderItem;
                    Product? product = poItem?.Product;

                    // fallback to find product by productId if navigation wasn't loaded
                    if (product == null)
                    {
                        product = await _context.Products.FindAsync(rec.ProductId);
                    }

                    if (product == null)
                        throw new InvalidOperationException($"Product for ReceivedStock {rec.Id} not found.");

                    // Decide quantities: for simplicity, treat posted quantity as canonical stored quantity.
                    decimal qty = rec.QuantityReceived;
                    decimal unitCost = poItem?.UnitCost ?? 0m;

                    // Create StockReceiveItem
                    var sri = new StockReceiveItem
                    {
                        StockReceive = stockReceive,
                        ProductId = product.Id,
                        FromUnitId = poItem?.UnitId,
                        QuantityInFromUnit = qty,
                        Quantity = qty, // if you have conversions, apply them here
                        UnitCost = unitCost,
                        BatchNumber = null,
                        ExpiryDate = null,
                        Remarks = rec.Notes
                    };
                    _context.StockReceiveItems.Add(sri);

                    // Create InventoryTransaction (positive quantity for StockIn)
                    var invTx = new InventoryTransaction
                    {
                        ProductId = product.Id,
                        ActionType = InventoryActionType.StockIn,
                        Quantity = qty,
                        UnitCost = unitCost,
                        ReferenceNumber = $"PO#{purchaseOrderId}:R#{rec.Id}",
                        Remarks = $"Posted from PO receive {rec.Id}",
                        TransactionDate = DateTime.UtcNow,
                        PerformedById = processedByUserId,
                        ReferenceId = null,
                        ReferenceType = "StockReceive"
                    };
                    _context.InventoryTransactions.Add(invTx);

                    product.OnHand += qty;
                    _context.Products.Update(product);

                    rec.Processed = true;
                    _context.ReceivedStocks.Update(rec);
                }
                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Failed to post received stock to inventory for PO {PoId}", purchaseOrderId);
                throw;
            }
        }

        public async Task<PurchaseOrderReadDto> CreateAsync(PurchaseOrderCreateDto dto, string createdByUserId)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            // Basic validations
            var supplier = await _context.Suppliers.FindAsync(dto.SupplierId);
            if (supplier == null) throw new InvalidOperationException("Supplier not found.");

            // Determine PO number: if provided in DTO use it (and enforce uniqueness); otherwise auto-generate one.
            string poNumber;
            if (!string.IsNullOrWhiteSpace(dto.PurchaseOrderNumber))
            {
                var exists = await _context.PurchaseOrders.AnyAsync(p => p.PurchaseOrderNumber == dto.PurchaseOrderNumber);
                if (exists) throw new InvalidOperationException("PurchaseOrderNumber already exists.");
                poNumber = dto.PurchaseOrderNumber!;
            }
            else
            {
                poNumber = await GetUniquePurchaseOrderNumberAsync();
            }

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var po = new PurchaseOrder
                {
                    SupplierId = dto.SupplierId,
                    PurchaseOrderNumber = poNumber,
                    OrderDate = dto.OrderDate ?? DateTime.UtcNow,
                    ExpectedDeliveryDate = dto.ExpectedDeliveryDate,
                    Remarks = dto.Remarks,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = createdByUserId,
                };

                _context.PurchaseOrders.Add(po);
                await _context.SaveChangesAsync(); // will set po.Id

                decimal totalCost = 0m;
                // Create items
                foreach (var itemDto in dto.Items)
                {
                    // Validate product exists
                    var product = await _context.Products.FindAsync(itemDto.ProductId);
                    if (product == null)
                        throw new InvalidOperationException($"Product id {itemDto.ProductId} not found.");

                    // Optional: validate Unit exists
                    var unit = await _context.Units.FindAsync(itemDto.UnitId);
                    if (unit == null)
                        throw new InvalidOperationException($"Unit id {itemDto.UnitId} not found.");

                    var item = new PurchaseOrderItem
                    {
                        PurchaseOrderId = po.Id,
                        ProductId = itemDto.ProductId,
                        UnitId = itemDto.UnitId,
                        QuantityOrdered = itemDto.QuantityOrdered,
                        QuantityReceived = 0m,
                        UnitCost = itemDto.UnitCost,
                        Remarks = itemDto.Remarks
                    };

                    _context.PurchaseOrderItems.Add(item);

                    totalCost += itemDto.QuantityOrdered * itemDto.UnitCost;
                }

                // Persist items
                await _context.SaveChangesAsync();

                // Update PO total cost
                po.TotalCost = totalCost;
                _context.PurchaseOrders.Update(po);
                await _context.SaveChangesAsync();

                await tx.CommitAsync();

                // Reload with includes to return the DTO
                var created = await _context.PurchaseOrders
                    .Include(p => p.Supplier)
                    .Include(p => p.Items).ThenInclude(i => i.Product)
                    .Include(p => p.Items).ThenInclude(i => i.Unit)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == po.Id);

                return _mapper.Map<PurchaseOrderReadDto>(created!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create PurchaseOrder {PoNumber}", poNumber);
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<PurchaseOrderReadDto> UpdateAsync(PurchaseOrderUpdateDto dto, string updatedByUserId)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var po = await _context.PurchaseOrders
                .Include(p => p.Items)
                .Include(p => p.ReceivedStocks).ThenInclude(rs => rs.PurchaseOrderItem).ThenInclude(pi => pi.Product)
                .FirstOrDefaultAsync(p => p.Id == dto.Id);

            if (po == null) throw new KeyNotFoundException("Purchase order not found.");

            // Validate supplier
            var supplier = await _context.Suppliers.FindAsync(dto.SupplierId);
            if (supplier == null) throw new InvalidOperationException("Supplier not found.");

            // Check unique PO number (exclude self)
            var exists = await _context.PurchaseOrders
                .AnyAsync(p => p.PurchaseOrderNumber == dto.PurchaseOrderNumber && p.Id != dto.Id);
            if (exists) throw new InvalidOperationException("PurchaseOrderNumber already exists.");

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // Update header
                po.SupplierId = dto.SupplierId;
                po.PurchaseOrderNumber = dto.PurchaseOrderNumber;
                po.ExpectedDeliveryDate = dto.ExpectedDeliveryDate;
                po.Remarks = dto.Remarks;
                // Optionally update CreatedBy/CreatedAt? Usually keep original.

                // Reconcile items: update existing, add new, remove missing
                var incomingById = dto.Items.Where(i => i.Id.HasValue).ToDictionary(i => i.Id!.Value);
                var existingItems = po.Items.ToList();

                // Update existing items
                foreach (var existing in existingItems)
                {
                    if (incomingById.TryGetValue(existing.Id, out var updDto))
                    {
                        // Update fields
                        existing.ProductId = updDto.ProductId;
                        existing.UnitId = updDto.UnitId;
                        existing.QuantityOrdered = updDto.QuantityOrdered;
                        // NOTE: do not reduce QuantityReceived here — that is a separate business action
                        existing.UnitCost = updDto.UnitCost;
                        existing.Remarks = updDto.Remarks;
                    }
                    else
                    {
                        // incoming does not contain this item -> delete it (only if no received qty)
                        if (existing.QuantityReceived > 0)
                            throw new InvalidOperationException("Cannot remove a line that has received quantity.");
                        _context.PurchaseOrderItems.Remove(existing);
                    }
                }

                // Add new items
                var newItems = dto.Items.Where(i => !i.Id.HasValue).ToList();
                foreach (var newDto in newItems)
                {
                    var product = await _context.Products.FindAsync(newDto.ProductId);
                    if (product == null) throw new InvalidOperationException($"Product id {newDto.ProductId} not found.");
                    var unit = await _context.Units.FindAsync(newDto.UnitId);
                    if (unit == null) throw new InvalidOperationException($"Unit id {newDto.UnitId} not found.");

                    var newItem = new PurchaseOrderItem
                    {
                        PurchaseOrderId = po.Id,
                        ProductId = newDto.ProductId,
                        UnitId = newDto.UnitId,
                        QuantityOrdered = newDto.QuantityOrdered,
                        QuantityReceived = 0m,
                        UnitCost = newDto.UnitCost,
                        Remarks = newDto.Remarks
                    };
                    _context.PurchaseOrderItems.Add(newItem);
                }

                // Recompute total cost
                await _context.SaveChangesAsync(); // persist item updates/adds/removals
                var refreshedItems = await _context.PurchaseOrderItems.Where(i => i.PurchaseOrderId == po.Id).ToListAsync();
                po.TotalCost = refreshedItems.Sum(i => i.QuantityOrdered * i.UnitCost);

                _context.PurchaseOrders.Update(po);
                await _context.SaveChangesAsync();

                await tx.CommitAsync();

                var updated = await _context.PurchaseOrders
                    .Include(p => p.Supplier)
                    .Include(p => p.Items).ThenInclude(i => i.Product)
                    .Include(p => p.Items).ThenInclude(i => i.Unit)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == po.Id);

                return _mapper.Map<PurchaseOrderReadDto>(updated!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update PurchaseOrder {PoId}", dto.Id);
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var po = await _context.PurchaseOrders
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (po == null) return false;

            // Business rule: do not delete PO that has received quantities (use cancel instead)
            if (po.Items.Any(i => i.QuantityReceived > 0))
                throw new InvalidOperationException("Cannot delete purchase order with received items.");

            _context.PurchaseOrderItems.RemoveRange(po.Items);
            _context.PurchaseOrders.Remove(po);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ReceivedStockReadDto> ReceiveStockAsync(ReceiveStockCreateDto dto, string receivedByUserId)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            // Validate PO + item exist
            var po = await _context.PurchaseOrders
                .Include(p => p.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(p => p.Id == dto.PurchaseOrderId);

            if (po == null) throw new InvalidOperationException("Purchase order not found.");

            var item = await _context.PurchaseOrderItems
                .FirstOrDefaultAsync(i => i.Id == dto.PurchaseOrderItemId && i.PurchaseOrderId == dto.PurchaseOrderId);

            if (item == null) throw new InvalidOperationException("Purchase order item not found.");

            var remaining = item.QuantityOrdered - item.QuantityReceived;
            if (dto.QuantityReceived <= 0 || dto.QuantityReceived > remaining)
                throw new InvalidOperationException($"QuantityReceived must be > 0 and <= remaining ({remaining}).");

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var rec = new ReceivedStock
                {
                    PurchaseOrderId = dto.PurchaseOrderId,
                    PurchaseOrderItemId = dto.PurchaseOrderItemId,
                    ProductId = item.ProductId,                // <-- set product id here
                    QuantityReceived = dto.QuantityReceived,
                    ReceivedDate = dto.ReceivedDate ?? DateTime.UtcNow,
                    ReferenceNumber = dto.ReferenceNumber,
                    Notes = dto.Notes,
                    ReceivedByUserId = receivedByUserId,
                    Processed = false
                };


                _context.ReceivedStocks.Add(rec);

                // adjust item received qty
                item.QuantityReceived += dto.QuantityReceived;

                // update PO status: compute aggregate received status
                var refreshedItems = po.Items; // includes items collection
                                               // we updated item.QuantityReceived in memory; recompute
                var anyReceived = refreshedItems.Any(i => i.QuantityReceived > 0);
                var allReceived = refreshedItems.All(i => i.QuantityReceived >= i.QuantityOrdered);

                po.Status = allReceived ? PurchaseOrderStatus.Received :
                            (anyReceived ? PurchaseOrderStatus.PartiallyReceived : po.Status);

                // optionally set IsReceived flag
                po.IsReceived = allReceived;

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                // reload created rec with navigation
                var created = await _context.ReceivedStocks
                    .Include(r => r.PurchaseOrderItem).ThenInclude(i => i.Product)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Id == rec.Id);

                return _mapper.Map<ReceivedStockReadDto>(created!);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Failed to receive stock for PO {PoId}", dto.PurchaseOrderId);
                throw;
            }
        }

        public async Task<bool> DeleteReceivedStockAsync(int receivedStockId)
        {
            var rec = await _context.ReceivedStocks
                .Include(r => r.PurchaseOrderItem)
                .FirstOrDefaultAsync(r => r.Id == receivedStockId);

            if (rec == null) return false;

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // adjust related item
                var item = rec.PurchaseOrderItem;
                if (item == null) throw new InvalidOperationException("Related purchase order item not found.");

                // Decrease received quantity but not below zero
                item.QuantityReceived = Math.Max(0, item.QuantityReceived - rec.QuantityReceived);

                _context.ReceivedStocks.Remove(rec);

                // update parent PO status
                var po = await _context.PurchaseOrders
                    .Include(p => p.Items)
                    .FirstOrDefaultAsync(p => p.Id == rec.PurchaseOrderId);

                if (po != null)
                {
                    var anyReceived = po.Items.Any(i => i.QuantityReceived > 0);
                    var allReceived = po.Items.All(i => i.QuantityReceived >= i.QuantityOrdered);
                    po.Status = allReceived ? PurchaseOrderStatus.Received :
                                (anyReceived ? PurchaseOrderStatus.PartiallyReceived : PurchaseOrderStatus.Draft);
                    po.IsReceived = allReceived;
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Failed to delete received stock {Id}", receivedStockId);
                throw;
            }
        }

        public async Task<bool> RemoveItemByIdAsync(int purchaseOrderItemId)
        {
            var item = await _context.PurchaseOrderItems
                .Include(i => i.PurchaseOrder)
                .FirstOrDefaultAsync(i => i.Id == purchaseOrderItemId);

            if (item == null) return false;

            if (item.QuantityReceived > 0)
                throw new InvalidOperationException("Cannot remove item that has received quantity.");

            _context.PurchaseOrderItems.Remove(item);

            // update PO total cost
            var po = item.PurchaseOrder;
            if (po != null)
            {
                var refreshedItems = await _context.PurchaseOrderItems
                    .Where(i => i.PurchaseOrderId == po.Id)
                    .ToListAsync();
                po.TotalCost = refreshedItems.Sum(i => i.QuantityOrdered * i.UnitCost);
            }

            await _context.SaveChangesAsync();
            return true;
        }


    }
}
