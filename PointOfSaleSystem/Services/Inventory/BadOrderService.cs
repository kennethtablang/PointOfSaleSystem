using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Inventory;
using PointOfSaleSystem.Enums;
using PointOfSaleSystem.Interfaces.Inventory;
using PointOfSaleSystem.Models.Inventory;

namespace PointOfSaleSystem.Services.Inventory
{
    public class BadOrderService : IBadOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<BadOrderService> _logger;

        public BadOrderService(ApplicationDbContext context, IMapper mapper, ILogger<BadOrderService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<BadOrderReadDto>> GetAllAsync()
        {
            var list = await _context.BadOrders
                .Include(b => b.Product)
                .Include(b => b.ReportedByUser)
                .OrderByDescending(b => b.BadOrderDate)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<BadOrderReadDto>>(list);
        }

        public async Task<BadOrderReadDto?> GetByIdAsync(int id)
        {
            var b = await _context.BadOrders
                .Include(b => b.Product)
                .Include(b => b.ReportedByUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

            return b == null ? null : _mapper.Map<BadOrderReadDto>(b);
        }

        /// <summary>
        /// Create a bad order, deduct product.OnHand, create an InventoryTransaction (negative quantity).
        /// </summary>
        public async Task<BadOrderReadDto> CreateAsync(BadOrderCreateDto dto, string reportedByUserId)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == dto.ProductId);
            if (product == null)
                throw new InvalidOperationException($"Product id {dto.ProductId} not found.");

            // Ensure OnHand is enough (business rule). Product.OnHand is decimal; dto.Quantity is int.
            if (product.OnHand < dto.Quantity)
                throw new InvalidOperationException($"Insufficient stock: product '{product.Name}' has {product.OnHand} on-hand, cannot deduct {dto.Quantity}.");

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // Map dto -> entity
                var badOrder = _mapper.Map<BadOrder>(dto);
                badOrder.ReportedByUserId = reportedByUserId;
                badOrder.BadOrderDate = DateTime.UtcNow;

                _context.BadOrders.Add(badOrder);

                // Deduct on-hand
                product.OnHand -= dto.Quantity;
                _context.Products.Update(product);

                // Persist to get badOrder.Id
                await _context.SaveChangesAsync();

                // Create inventory transaction for audit (negative quantity)
                var invTx = new InventoryTransaction
                {
                    ProductId = product.Id,
                    // Use an enum member for stock-out. Adjust the enum member if different in your project.
                    ActionType = InventoryActionType.BadOrder,
                    Quantity = -dto.Quantity,
                    UnitCost = null,
                    ReferenceNumber = $"BAD-{badOrder.Id}",
                    Remarks = $"Bad order: {dto.Reason}. {dto.Remarks}",
                    TransactionDate = DateTime.UtcNow,
                    PerformedById = reportedByUserId,
                    ReferenceId = badOrder.Id,
                    ReferenceType = "BadOrder"
                };

                _context.InventoryTransactions.Add(invTx);
                await _context.SaveChangesAsync();

                // Link inventory transaction id to bad order (optional, useful for traceability)
                badOrder.InventoryTransactionId = invTx.Id;
                _context.BadOrders.Update(badOrder);
                await _context.SaveChangesAsync();

                await tx.CommitAsync();

                var created = await _context.BadOrders
                    .Include(b => b.Product)
                    .Include(b => b.ReportedByUser)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == badOrder.Id);

                return _mapper.Map<BadOrderReadDto>(created!);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Failed to create bad order for product {ProductId}", dto.ProductId);
                throw;
            }
        }

        /// <summary>
        /// Update a bad order. This method will reconcile Product.OnHand by applying the difference (newQuantity - oldQuantity).
        /// It will create a compensating InventoryTransaction to record the adjustment.
        /// </summary>
        public async Task<BadOrderReadDto?> UpdateAsync(int id, BadOrderUpdateDto dto)
        {
            var existing = await _context.BadOrders
                .Include(b => b.Product)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (existing == null) return null;

            var product = existing.Product ?? await _context.Products.FirstOrDefaultAsync(p => p.Id == existing.ProductId);
            if (product == null)
                throw new InvalidOperationException($"Product id {existing.ProductId} not found.");

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // compute delta: positive => increase bad quantity (further reduce OnHand)
                var delta = dto.Quantity - existing.Quantity;

                if (delta > 0)
                {
                    // need to deduct additional stock
                    if (product.OnHand < delta)
                        throw new InvalidOperationException($"Insufficient stock: product '{product.Name}' has {product.OnHand} on-hand, cannot deduct additional {delta}.");
                    product.OnHand -= delta;
                }
                else if (delta < 0)
                {
                    // restore some stock back
                    product.OnHand += Math.Abs(delta);
                }

                // update product
                _context.Products.Update(product);

                // apply fields from dto to existing bad order (mapper)
                _mapper.Map(dto, existing);
                existing.BadOrderDate = dto.BadOrderDate;

                _context.BadOrders.Update(existing);
                await _context.SaveChangesAsync();

                // create a compensating inventory transaction to record this adjustment (if delta != 0)
                if (delta != 0)
                {
                    var adjTx = new InventoryTransaction
                    {
                        ProductId = product.Id,
                        ActionType = delta < 0 ? InventoryActionType.StockIn : InventoryActionType.BadOrder,
                        Quantity = delta < 0 ? Math.Abs(delta) : -Math.Abs(delta),
                        UnitCost = null,
                        ReferenceNumber = $"BAD-ADJ-{existing.Id}",
                        Remarks = $"BadOrder update adjustment (delta {delta}).",
                        TransactionDate = DateTime.UtcNow,
                        PerformedById = existing.ReportedByUserId,
                        ReferenceId = existing.Id,
                        ReferenceType = "BadOrderAdjustment"
                    };

                    _context.InventoryTransactions.Add(adjTx);
                    await _context.SaveChangesAsync();

                    // Optionally update BadOrder.InventoryTransactionId to point to latest adjustment (or leave original)
                    existing.InventoryTransactionId = adjTx.Id;
                    _context.BadOrders.Update(existing);
                    await _context.SaveChangesAsync();
                }

                await tx.CommitAsync();

                var updated = await _context.BadOrders
                    .Include(b => b.Product)
                    .Include(b => b.ReportedByUser)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == existing.Id);

                return _mapper.Map<BadOrderReadDto>(updated!);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Failed to update bad order {BadOrderId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a bad order. NOTE: This implementation does NOT automatically restore product.OnHand.
        /// If you want deletion to restore stock, modify this method to add back the quantity and create a reversing InventoryTransaction.
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.BadOrders.FirstOrDefaultAsync(b => b.Id == id);
            if (existing == null) return false;

            _context.BadOrders.Remove(existing);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
