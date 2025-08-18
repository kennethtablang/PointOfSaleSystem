using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PointOfSaleSystem.Data;
using PointOfSaleSystem.DTOs.Inventory;
using PointOfSaleSystem.Enums;
using PointOfSaleSystem.Interfaces.Inventory;
using PointOfSaleSystem.Models.Inventory;

namespace PointOfSaleSystem.Services.Inventory
{
    public class InventoryTransactionService : IInventoryTransactionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public InventoryTransactionService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InventoryTransactionReadDto>> GetAllAsync(int? productId = null, InventoryActionType? actionType = null, DateTime? from = null, DateTime? to = null)
        {
            var q = _context.InventoryTransactions
                .Include(t => t.Product)
                .Include(t => t.PerformedByUser)
                .AsNoTracking()
                .AsQueryable();

            if (productId.HasValue)
                q = q.Where(t => t.ProductId == productId.Value);

            if (actionType.HasValue)
                q = q.Where(t => t.ActionType == actionType.Value);

            if (from.HasValue)
                q = q.Where(t => t.TransactionDate >= from.Value);

            if (to.HasValue)
                q = q.Where(t => t.TransactionDate <= to.Value);

            var list = await q.OrderByDescending(t => t.TransactionDate).ToListAsync();
            return _mapper.Map<IEnumerable<InventoryTransactionReadDto>>(list);
        }
        public async Task<InventoryTransactionReadDto?> GetByIdAsync(int id)
        {
            var t = await _context.InventoryTransactions
                .Include(x => x.Product)
                .Include(x => x.PerformedByUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (t == null) return null;
            return _mapper.Map<InventoryTransactionReadDto>(t);
        }

        public async Task<InventoryTransactionReadDto> CreateAsync(InventoryTransactionCreateDto dto, string performedByUserId)
        {
            // Validate product exists
            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null) throw new KeyNotFoundException("Product not found");

            // Determine signed quantity based on action type (server convenience).
            var signedQty = GetSignedQuantity(dto.Quantity, dto.ActionType);

            var entity = _mapper.Map<InventoryTransaction>(dto);
            entity.Quantity = signedQty;
            entity.PerformedById = performedByUserId;
            entity.TransactionDate = dto.TransactionDate ?? DateTime.Now;

            await _context.InventoryTransactions.AddAsync(entity);
            await _context.SaveChangesAsync();

            // fetch with includes for mapping
            var created = await _context.InventoryTransactions
                .Include(x => x.Product)
                .Include(x => x.PerformedByUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == entity.Id);

            return _mapper.Map<InventoryTransactionReadDto>(created!);
        }

        public async Task<bool> UpdateAsync(int id, InventoryTransactionUpdateDto dto)
        {
            var entity = await _context.InventoryTransactions.FindAsync(id);
            if (entity == null) return false;

            // Update allowed fields: Quantity (with respect to action type), UnitCost, ReferenceNumber, Remarks, TransactionDate
            // We do NOT change ProductId or ActionType here for safety.
            if (dto.TransactionDate.HasValue)
                entity.TransactionDate = dto.TransactionDate.Value;

            entity.UnitCost = dto.UnitCost;
            entity.ReferenceNumber = dto.ReferenceNumber;
            entity.Remarks = dto.Remarks;

            // quantity handling:
            if (dto.Quantity != entity.Quantity)
            {
                // For Adjustment and Transfer we accept whatever sign client gives.
                if (entity.ActionType == InventoryActionType.Adjustment || entity.ActionType == InventoryActionType.Transfer)
                {
                    entity.Quantity = dto.Quantity;
                }
                else
                {
                    // For other action types, enforce sign rules
                    var normalized = GetSignedQuantity(dto.Quantity, entity.ActionType);
                    entity.Quantity = normalized;
                }
            }

            _context.Entry(entity).State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.InventoryTransactions.FindAsync(id);
            if (entity == null) return false;

            _context.InventoryTransactions.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<decimal> GetProductOnHandAsync(int productId)
        {
            // Sum of all signed quantities for product
            var sum = await _context.InventoryTransactions
                .Where(t => t.ProductId == productId)
                .SumAsync(t => (decimal?)t.Quantity);

            return sum ?? 0m;
        }

        public async Task<ProductStockDto> GetProductStockAsync(int productId)
        {
            var onHand = await GetProductOnHandAsync(productId);
            // Reserved currently not implemented — returned as 0
            return new ProductStockDto
            {
                ProductId = productId,
                OnHand = onHand,
                Reserved = 0m
            };
        }

        // ---------------- Helper ----------------
        /// <summary>
        /// Convert an absolute quantity and an action type into a signed quantity
        /// using server-side rules. Clients can pass positive amounts; server will
        /// set sign according to action semantics.
        /// </summary>
        private static decimal GetSignedQuantity(decimal suppliedQty, InventoryActionType actionType)
        {
            var abs = Math.Abs(suppliedQty);

            switch (actionType)
            {
                case InventoryActionType.StockIn:
                case InventoryActionType.Return:
                case InventoryActionType.VoidedSale:
                    // inbound increases stock
                    return abs;

                case InventoryActionType.Sale:
                case InventoryActionType.BadOrder:
                    // outbound decreases stock
                    return -abs;

                case InventoryActionType.Adjustment:
                case InventoryActionType.Transfer:
                default:
                    // For ambiguous actions, respect the supplied sign (allow positive or negative)
                    // i.e., client can pass negative for decrease, positive for increase
                    return suppliedQty;
            }
        }
    }
}
