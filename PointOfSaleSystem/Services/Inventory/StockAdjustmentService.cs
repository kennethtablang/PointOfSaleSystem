using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Inventory;
using PointOfSaleSystem.Enums;
using PointOfSaleSystem.Interfaces.Inventory;
using PointOfSaleSystem.Models.Inventory;

namespace PointOfSaleSystem.Services.Inventory
{
    public class StockAdjustmentService : IStockAdjustmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<StockAdjustmentService> _logger;

        public StockAdjustmentService(ApplicationDbContext context, IMapper mapper, ILogger<StockAdjustmentService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<StockAdjustmentListDto>> GetAllAsync()
        {
            var adjustments = await _context.StockAdjustments
                .Include(sa => sa.Product)
                .Include(sa => sa.Unit)
                .Include(sa => sa.AdjustedByUser)
                .OrderByDescending(sa => sa.AdjustmentDate)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<StockAdjustmentListDto>>(adjustments);
        }

        public async Task<StockAdjustmentReadDto?> GetByIdAsync(int id)
        {
            var adjustment = await _context.StockAdjustments
                .Include(sa => sa.Product)
                .Include(sa => sa.Unit)
                .Include(sa => sa.AdjustedByUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(sa => sa.Id == id);

            return adjustment == null ? null : _mapper.Map<StockAdjustmentReadDto>(adjustment);
        }

        public async Task<StockAdjustmentReadDto> CreateAsync(StockAdjustmentCreateDto dto, string userId)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Quantity == 0) throw new InvalidOperationException("Quantity must not be zero.");

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == dto.ProductId);
            if (product == null) throw new InvalidOperationException($"Product id {dto.ProductId} not found.");

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // map -> entity
                var adjustment = _mapper.Map<StockAdjustment>(dto);
                adjustment.AdjustmentDate = DateTime.UtcNow;
                adjustment.AdjustedByUserId = userId;

                // apply to product.OnHand
                // Positive quantity -> increase (StockIn); Negative -> decrease (StockOut)
                var newOnHand = product.OnHand + adjustment.Quantity;

                // Business rule: if decreasing, ensure not below zero
                if (newOnHand < 0)
                {
                    throw new InvalidOperationException($"Insufficient stock: product '{product.Name}' has {product.OnHand} on-hand; adjustment {adjustment.Quantity} would result in negative on-hand.");
                }

                product.OnHand = newOnHand;

                _context.StockAdjustments.Add(adjustment);
                _context.Products.Update(product);

                await _context.SaveChangesAsync();

                // create InventoryTransaction for audit
                var invTx = new InventoryTransaction
                {
                    ProductId = product.Id,
                    ActionType = adjustment.Quantity > 0 ? InventoryActionType.StockIn : InventoryActionType.BadOrder /* or StockOut/Adjustment */,
                    Quantity = adjustment.Quantity,
                    UnitCost = null,
                    ReferenceNumber = $"ADJ-{adjustment.Id}",
                    Remarks = adjustment.Reason,
                    TransactionDate = DateTime.UtcNow,
                    PerformedById = userId,
                    ReferenceId = adjustment.Id,
                    ReferenceType = "StockAdjustment"
                };

                _context.InventoryTransactions.Add(invTx);
                await _context.SaveChangesAsync();

                // link transaction id
                adjustment.InventoryTransactionId = invTx.Id;
                _context.StockAdjustments.Update(adjustment);
                await _context.SaveChangesAsync();

                await tx.CommitAsync();

                var created = await _context.StockAdjustments
                    .Include(sa => sa.Product)
                    .Include(sa => sa.Unit)
                    .Include(sa => sa.AdjustedByUser)
                    .AsNoTracking()
                    .FirstAsync(sa => sa.Id == adjustment.Id);

                return _mapper.Map<StockAdjustmentReadDto>(created);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Failed to create stock adjustment for ProductId {ProductId}", dto.ProductId);
                throw;
            }
        }

        public async Task<StockAdjustmentReadDto?> UpdateAsync(int id, StockAdjustmentUpdateDto dto, string userId)
        {
            var adjustment = await _context.StockAdjustments
                .Include(sa => sa.Product)
                .FirstOrDefaultAsync(sa => sa.Id == id);

            if (adjustment == null) return null;
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Quantity == 0) throw new InvalidOperationException("Quantity must not be zero.");

            var product = adjustment.Product ?? await _context.Products.FirstOrDefaultAsync(p => p.Id == adjustment.ProductId);
            if (product == null) throw new InvalidOperationException($"Product id {adjustment.ProductId} not found.");

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // compute delta: new - old
                var oldQty = adjustment.Quantity;
                var delta = dto.Quantity - oldQty;

                // ensure not cause negative onhand
                if (delta < 0 && product.OnHand + delta < 0)
                {
                    throw new InvalidOperationException($"Insufficient stock: product '{product.Name}' has {product.OnHand} on-hand; reducing by {Math.Abs(delta)} would result negative.");
                }

                // apply delta
                product.OnHand += delta;
                _context.Products.Update(product);

                // update adjustment fields
                _mapper.Map(dto, adjustment);
                adjustment.AdjustmentDate = DateTime.UtcNow;
                adjustment.AdjustedByUserId = userId;

                _context.StockAdjustments.Update(adjustment);
                await _context.SaveChangesAsync();

                // create a compensating inventory transaction for the delta (if non-zero)
                if (delta != 0)
                {
                    var adjTx = new InventoryTransaction
                    {
                        ProductId = product.Id,
                        ActionType = delta > 0 ? InventoryActionType.StockIn : InventoryActionType.BadOrder /* or StockOut/Adjustment */,
                        Quantity = delta,
                        UnitCost = null,
                        ReferenceNumber = $"ADJ-UPD-{adjustment.Id}",
                        Remarks = $"StockAdjustment update delta {delta}",
                        TransactionDate = DateTime.UtcNow,
                        PerformedById = userId,
                        ReferenceId = adjustment.Id,
                        ReferenceType = "StockAdjustmentUpdate"
                    };

                    _context.InventoryTransactions.Add(adjTx);
                    await _context.SaveChangesAsync();

                    adjustment.InventoryTransactionId = adjTx.Id;
                    _context.StockAdjustments.Update(adjustment);
                    await _context.SaveChangesAsync();
                }

                await tx.CommitAsync();

                var updated = await _context.StockAdjustments
                    .Include(sa => sa.Product)
                    .Include(sa => sa.Unit)
                    .Include(sa => sa.AdjustedByUser)
                    .AsNoTracking()
                    .FirstAsync(sa => sa.Id == adjustment.Id);

                return _mapper.Map<StockAdjustmentReadDto>(updated);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Failed to update stock adjustment {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var adjustment = await _context.StockAdjustments
                .Include(sa => sa.Product)
                .FirstOrDefaultAsync(sa => sa.Id == id);

            if (adjustment == null) return false;

            var product = adjustment.Product ?? await _context.Products.FirstOrDefaultAsync(p => p.Id == adjustment.ProductId);
            if (product == null) throw new InvalidOperationException($"Product id {adjustment.ProductId} not found.");

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // Revert product.OnHand by subtracting the adjustment (i.e. OnHand -= adjustment.Quantity)
                // If the original adjustment was positive (stock in), we remove it -> subtract; if negative, removing it adds back.
                var newOnHand = product.OnHand - adjustment.Quantity;
                if (newOnHand < 0)
                {
                    throw new InvalidOperationException($"Cannot delete adjustment: product '{product.Name}' would have negative on-hand after revert.");
                }

                product.OnHand = newOnHand;
                _context.Products.Update(product);

                // create reversing inventory transaction for audit
                var revTx = new InventoryTransaction
                {
                    ProductId = product.Id,
                    ActionType = adjustment.Quantity > 0 ? InventoryActionType.BadOrder /* or StockOut/Adjustment */ : InventoryActionType.StockIn,
                    Quantity = -adjustment.Quantity,
                    UnitCost = null,
                    ReferenceNumber = $"ADJ-DEL-{adjustment.Id}",
                    Remarks = $"Revert StockAdjustment deletion",
                    TransactionDate = DateTime.UtcNow,
                    PerformedById = adjustment.AdjustedByUserId,
                    ReferenceId = adjustment.Id,
                    ReferenceType = "StockAdjustmentDelete"
                };

                _context.InventoryTransactions.Add(revTx);

                // remove adjustment
                _context.StockAdjustments.Remove(adjustment);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Failed to delete stock adjustment {Id}", id);
                throw;
            }
        }
    }
}
