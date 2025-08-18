using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Inventory;
using PointOfSaleSystem.Enums;
using PointOfSaleSystem.Interfaces.Inventory;
using PointOfSaleSystem.Models.Inventory;

namespace PointOfSaleSystem.Services.Inventory
{
    public class StockReceiveService : IStockReceiveService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<StockReceiveService> _logger;

        public StockReceiveService(ApplicationDbContext context, IMapper mapper, ILogger<StockReceiveService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<StockReceiveReadDto>> GetAllAsync()
        {
            var list = await _context.StockReceives
                .Include(sr => sr.Items).ThenInclude(i => i.Product)
                .Include(sr => sr.ReceivedByUser)
                .Include(sr => sr.PurchaseOrder)
                .AsNoTracking()
                .OrderByDescending(sr => sr.ReceivedDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<StockReceiveReadDto>>(list);
        }

        public async Task<StockReceiveReadDto?> GetByIdAsync(int id)
        {
            var rec = await _context.StockReceives
                .Include(sr => sr.Items).ThenInclude(i => i.Product)
                .Include(sr => sr.ReceivedByUser)
                .Include(sr => sr.PurchaseOrder)
                .AsNoTracking()
                .FirstOrDefaultAsync(sr => sr.Id == id);

            return rec == null ? null : _mapper.Map<StockReceiveReadDto>(rec);
        }

        public async Task<IEnumerable<StockReceiveReadDto>> GetByPurchaseOrderIdAsync(int purchaseOrderId)
        {
            var list = await _context.StockReceives
                .Where(sr => sr.PurchaseOrderId == purchaseOrderId)
                .Include(sr => sr.Items).ThenInclude(i => i.Product)
                .Include(sr => sr.ReceivedByUser)
                .Include(sr => sr.PurchaseOrder)
                .AsNoTracking()
                .OrderByDescending(sr => sr.ReceivedDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<StockReceiveReadDto>>(list);
        }

        public async Task<StockReceiveReadDto> CreateFromReceivedStocksAsync(int purchaseOrderId, string processedByUserId, bool allowOverReceive = false)
        {
            if (string.IsNullOrWhiteSpace(processedByUserId))
                throw new ArgumentNullException(nameof(processedByUserId));

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // Load PO + unprocessed ReceivedStock rows
                var po = await _context.PurchaseOrders
                    .Include(p => p.Items)
                    .FirstOrDefaultAsync(p => p.Id == purchaseOrderId);

                if (po == null) throw new InvalidOperationException("Purchase order not found.");

                var receivedRows = await _context.ReceivedStocks
                    .Include(r => r.PurchaseOrderItem).ThenInclude(pi => pi.Product)
                    .Where(r => r.PurchaseOrderId == purchaseOrderId && !r.Processed)
                    .ToListAsync();

                if (receivedRows == null || !receivedRows.Any())
                    throw new InvalidOperationException("No unprocessed received stock found for this purchase order.");

                // Optionally validate over-receive rules here. For now we will allow unless explicitly denied.
                if (!allowOverReceive)
                {
                    // simple check: ensure for each ReceivedStock we are not exceeding line remaining
                    foreach (var r in receivedRows)
                    {
                        var item = r.PurchaseOrderItem ?? await _context.PurchaseOrderItems.FirstOrDefaultAsync(i => i.Id == r.PurchaseOrderItemId);
                        if (item == null) throw new InvalidOperationException($"Related PO item {r.PurchaseOrderItemId} not found for ReceivedStock {r.Id}.");
                        var remaining = item.QuantityOrdered - item.QuantityReceived; // note: item.QuantityReceived already updated when ReceiveStockAsync ran
                        if (r.QuantityReceived > (remaining + 0.0000001m)) // small leeway for decimals
                        {
                            throw new InvalidOperationException($"ReceivedStock #{r.Id} quantity ({r.QuantityReceived}) exceeds remaining for PO item #{item.Id} ({remaining}).");
                        }
                    }
                }

                // Create StockReceive header
                var stockReceive = new StockReceive
                {
                    PurchaseOrderId = purchaseOrderId,
                    ReceivedDate = DateTime.UtcNow,
                    ReceivedByUserId = processedByUserId,
                    ReferenceNumber = $"GRN-{purchaseOrderId}-{DateTime.UtcNow:yyyyMMddHHmmss}",
                    Remarks = $"Posted from ReceivedStock for PO #{purchaseOrderId}"
                };

                // Add items and update product on-hand + create inventory transactions
                foreach (var r in receivedRows)
                {
                    // determine product (prefer navigation)
                    var product = r.PurchaseOrderItem?.Product ?? await _context.Products.FirstOrDefaultAsync(p => p.Id == r.ProductId);
                    if (product == null) throw new InvalidOperationException($"Product for ReceivedStock {r.Id} not found.");

                    var qty = r.QuantityReceived;

                    var item = new StockReceiveItem
                    {
                        ProductId = product.Id,
                        Quantity = qty,
                        QuantityInFromUnit = qty, // simple mapping; if you need conversion, alter here
                        UnitCost = r.PurchaseOrderItem?.UnitCost,
                        BatchNumber = null,
                        ExpiryDate = null,
                        Remarks = r.Notes
                    };

                    stockReceive.Items.Add(item);

                    // Increase on-hand
                    product.OnHand += qty;
                    _context.Products.Update(product);

                    // mark received row as processed
                    r.Processed = true;
                    _context.ReceivedStocks.Update(r);

                    // create inventory transaction (StockIn)
                    var inv = new InventoryTransaction
                    {
                        ProductId = product.Id,
                        ActionType = InventoryActionType.StockIn,
                        Quantity = qty,
                        UnitCost = item.UnitCost,
                        ReferenceNumber = $"PO#{purchaseOrderId}:R#{r.Id}",
                        Remarks = $"Posted ReceivedStock {r.Id} to inventory",
                        TransactionDate = DateTime.UtcNow,
                        PerformedById = processedByUserId,
                        ReferenceId = purchaseOrderId,
                        ReferenceType = "StockReceive"
                    };

                    _context.InventoryTransactions.Add(inv);

                    // (we don't set StockReceiveItem.InventoryTransactionId here because the item doesn't have an id yet.
                    // After SaveChanges the InventoryTransaction and StockReceiveItem will exist; if you want to link them,
                    // do a subsequent update to set InventoryTransactionId on the created StockReceiveItem)
                }

                _context.StockReceives.Add(stockReceive);

                // Persist all changes in one transaction
                await _context.SaveChangesAsync();

                // OPTIONAL: Link inventory transactions to stock receive items if you want the relationship stored.
                // (left as future enhancement)

                await tx.CommitAsync();

                // reload with navigation for DTO mapping
                var created = await _context.StockReceives
                    .Include(sr => sr.Items).ThenInclude(i => i.Product)
                    .Include(sr => sr.ReceivedByUser)
                    .Include(sr => sr.PurchaseOrder)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(sr => sr.Id == stockReceive.Id);

                return _mapper.Map<StockReceiveReadDto>(created!);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Failed to create StockReceive from ReceivedStock for PO {PoId}", purchaseOrderId);
                throw;
            }
        }

        public async Task DeleteAsync(int stockReceiveId)
        {
            var sr = await _context.StockReceives
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == stockReceiveId);

            if (sr == null) throw new KeyNotFoundException("StockReceive not found.");

            // NOTE: Deleting a stock receive does not automatically revert OnHand or InventoryTransaction.
            // If you want automatic reversal, implement logic here to decrement OnHand and remove transactions.
            _context.StockReceiveItems.RemoveRange(sr.Items);
            _context.StockReceives.Remove(sr);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.StockReceives.AnyAsync(sr => sr.Id == id);
        }
    }
}
