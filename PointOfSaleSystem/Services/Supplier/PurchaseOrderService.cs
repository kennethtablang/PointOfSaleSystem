using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Suppliers;
using PointOfSaleSystem.Interfaces.Supplier;
using PointOfSaleSystem.Models.Suppliers;

namespace PointOfSaleSystem.Services.Supplier
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PurchaseOrderService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ----------------------------
        // Purchase Orders
        // ----------------------------
        public async Task<IEnumerable<PurchaseOrderReadDto>> GetAllAsync()
        {
            var list = await _context.PurchaseOrders
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems).ThenInclude(pi => pi.Product)
                .Include(p => p.ReceivedStocks).ThenInclude(rs => rs.Product)
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PurchaseOrderReadDto>>(list);
        }

        public async Task<PurchaseOrderReadDto?> GetByIdAsync(int id)
        {
            var po = await _context.PurchaseOrders
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems).ThenInclude(pi => pi.Product)
                .Include(p => p.ReceivedStocks).ThenInclude(rs => rs.ReceivedByUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (po == null) return null;
            return _mapper.Map<PurchaseOrderReadDto>(po);
        }

        public async Task<PurchaseOrderReadDto> CreateAsync(PurchaseOrderCreateDto dto, string userId)
        {
            // Map DTO to entity
            var po = _mapper.Map<PurchaseOrder>(dto);

            // Set metadata
            po.CreatedAt = DateTime.Now;
            po.CreatedByUserId = userId;
            // generate PO number (simple scheme: PO-{timestamp}-{random})
            po.PurchaseOrderNumber = GeneratePurchaseOrderNumber();

            // Compute TotalCost from provided items (if any)
            if (po.PurchaseItems != null && po.PurchaseItems.Any())
            {
                po.TotalCost = po.PurchaseItems.Sum(i => i.CostPerUnit * i.Quantity);
            }
            else
            {
                po.TotalCost = 0m;
            }

            // Persist
            _context.PurchaseOrders.Add(po);
            await _context.SaveChangesAsync();

            // Return DTO (map with related names)
            var created = await _context.PurchaseOrders
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems).ThenInclude(pi => pi.Product)
                .Include(p => p.ReceivedStocks)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == po.Id);

            return _mapper.Map<PurchaseOrderReadDto>(created!);
        }

        public async Task<bool> UpdateAsync(int id, PurchaseOrderUpdateDto dto)
        {
            var po = await _context.PurchaseOrders.FindAsync(id);
            if (po == null) return false;

            // Map fields from DTO to entity (excluding collections)
            _mapper.Map(dto, po);

            // Recompute total cost if needed (we assume items may have changed separately)
            var items = await _context.PurchaseItems
                .Where(pi => pi.PurchaseOrderId == id)
                .ToListAsync();

            po.TotalCost = items.Sum(i => i.CostPerUnit * i.Quantity);

            _context.Entry(po).State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var po = await _context.PurchaseOrders
                .Include(p => p.PurchaseItems)
                .Include(p => p.ReceivedStocks)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (po == null) return false;

            // Optional: prevent deleting if any received stocks exist
            if (po.ReceivedStocks != null && po.ReceivedStocks.Any())
            {
                // business rule: don't delete POs with received stock
                return false;
            }

            // Remove items first (cascade may handle this but be explicit)
            if (po.PurchaseItems != null && po.PurchaseItems.Any())
            {
                _context.PurchaseItems.RemoveRange(po.PurchaseItems);
            }

            _context.PurchaseOrders.Remove(po);
            return await _context.SaveChangesAsync() > 0;
        }

        // ----------------------------
        // Purchase Items
        // ----------------------------
        public async Task<PurchaseItemReadDto> AddPurchaseItemAsync(int purchaseOrderId, PurchaseItemCreateDto dto)
        {
            var po = await _context.PurchaseOrders.FindAsync(purchaseOrderId);
            if (po == null) throw new KeyNotFoundException("Purchase order not found");

            var item = _mapper.Map<PurchaseItem>(dto);
            item.PurchaseOrderId = purchaseOrderId;
            item.ReceivedQuantity = item.ReceivedQuantity ?? 0;

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.PurchaseItems.Add(item);
                // update PO total
                po.TotalCost += item.CostPerUnit * item.Quantity;

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                // return read dto (include product name)
                var added = await _context.PurchaseItems
                    .Include(pi => pi.Product)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(pi => pi.Id == item.Id);

                return _mapper.Map<PurchaseItemReadDto>(added!);
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdatePurchaseItemAsync(int id, PurchaseItemUpdateDto dto)
        {
            var item = await _context.PurchaseItems.FindAsync(id);
            if (item == null) return false;

            // Load parent PO to adjust totals
            var po = await _context.PurchaseOrders.FindAsync(item.PurchaseOrderId);
            if (po == null) return false;

            // compute old total line cost
            var oldLine = item.CostPerUnit * item.Quantity;

            // map updates
            _mapper.Map(dto, item);

            // compute new line and adjust PO total
            var newLine = item.CostPerUnit * item.Quantity;
            po.TotalCost = (po.TotalCost - oldLine) + newLine;

            _context.Entry(item).State = EntityState.Modified;
            _context.Entry(po).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemovePurchaseItemAsync(int id)
        {
            var item = await _context.PurchaseItems.FindAsync(id);
            if (item == null) return false;

            var po = await _context.PurchaseOrders.FindAsync(item.PurchaseOrderId);
            if (po == null) return false;

            // prevent removing an item that already has received quantity > 0
            if (item.ReceivedQuantity.HasValue && item.ReceivedQuantity.Value > 0)
            {
                // business rule: cannot remove line if already received some stock
                return false;
            }

            // adjust PO total
            po.TotalCost -= item.CostPerUnit * item.Quantity;

            _context.PurchaseItems.Remove(item);
            _context.Entry(po).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;
        }

        // ----------------------------
        // Received Stocks
        // ----------------------------
        public async Task<ReceivedStockReadDto> AddReceivedStockAsync(ReceivedStockCreateDto dto, string userId)
        {
            // Validate PO and item existence
            var po = await _context.PurchaseOrders
                .Include(p => p.PurchaseItems)
                .FirstOrDefaultAsync(p => p.Id == dto.PurchaseOrderId);

            if (po == null) throw new KeyNotFoundException("Purchase order not found");

            var item = po.PurchaseItems?.FirstOrDefault(pi => pi.ProductId == dto.ProductId);
            if (item == null) throw new KeyNotFoundException("Purchase item not found on purchase order");

            // Check remaining quantity
            var alreadyReceived = item.ReceivedQuantity ?? 0;
            var remaining = item.Quantity - alreadyReceived;
            if (dto.QuantityReceived > remaining)
            {
                throw new InvalidOperationException("Quantity received exceeds remaining quantity for this item.");
            }

            var rs = _mapper.Map<ReceivedStock>(dto);
            rs.ReceivedByUserId = userId;
            rs.ReceivedDate = dto.ReceivedDate ?? DateTime.Now;

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.ReceivedStocks.Add(rs);

                // update PurchaseItem.ReceivedQuantity
                item.ReceivedQuantity = (item.ReceivedQuantity ?? 0) + dto.QuantityReceived;

                // Optional: update InventoryTransactionId linking here if you create inventory transaction separately

                // If all items fully received, mark PO.IsReceived true
                var allReceived = po.PurchaseItems != null && po.PurchaseItems.All(pi => (pi.ReceivedQuantity ?? 0) >= pi.Quantity);
                if (allReceived)
                {
                    po.IsReceived = true;
                    po.Status = PointOfSaleSystem.Enums.PurchaseOrderStatus.Received;
                }

                // Save changes
                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                var created = await _context.ReceivedStocks
                    .Include(r => r.Product)
                    .Include(r => r.ReceivedByUser)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Id == rs.Id);

                return _mapper.Map<ReceivedStockReadDto>(created!);
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteReceivedStockAsync(int id)
        {
            var rs = await _context.ReceivedStocks.FindAsync(id);
            if (rs == null) return false;

            // Find the corresponding purchase item
            var item = await _context.PurchaseItems
                .FirstOrDefaultAsync(pi => pi.PurchaseOrderId == rs.PurchaseOrderId && pi.ProductId == rs.ProductId);

            if (item == null) return false;

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // decrease received quantity (ensure non-negative)
                item.ReceivedQuantity = Math.Max(0, (item.ReceivedQuantity ?? 0) - rs.QuantityReceived);

                // If we deleted a received record, PO.IsReceived should be re-evaluated
                var po = await _context.PurchaseOrders
                    .Include(p => p.PurchaseItems)
                    .FirstOrDefaultAsync(p => p.Id == rs.PurchaseOrderId);

                if (po != null)
                {
                    po.IsReceived = !(po.PurchaseItems != null && po.PurchaseItems.Any(pi => (pi.ReceivedQuantity ?? 0) < pi.Quantity));
                    if (!po.IsReceived)
                    {
                        po.Status = PointOfSaleSystem.Enums.PurchaseOrderStatus.PartiallyReceived;
                    }
                }

                _context.ReceivedStocks.Remove(rs);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        // ----------------------------
        // Helper
        // ----------------------------
        private string GeneratePurchaseOrderNumber()
        {
            // Basic: PO-YYYYMMDD-HHMMSS-XXXX
            var stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var rand = new Random().Next(1000, 9999);
            return $"PO-{stamp}-{rand}";
        }
    }
}
