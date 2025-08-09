using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSystem.DTOs.Suppliers;
using PointOfSaleSystem.Interfaces.Supplier;
using System.Security.Claims;

namespace PointOfSaleSystem.Controllers.Supplier
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager")]
    public class PurchaseOrderController : Controller
    {
        private readonly IPurchaseOrderService _purchaseOrderService;

        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }

        // ----------------------------
        // Purchase Orders
        // ----------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseOrderReadDto>>> GetAll()
        {
            var result = await _purchaseOrderService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseOrderReadDto>> GetById(int id)
        {
            var result = await _purchaseOrderService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<PurchaseOrderReadDto>> Create([FromBody] PurchaseOrderCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
            var created = await _purchaseOrderService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PurchaseOrderUpdateDto dto)
        {
            var updated = await _purchaseOrderService.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _purchaseOrderService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        // ----------------------------
        // Purchase Items
        // ----------------------------
        [HttpPost("{purchaseOrderId}/items")]
        public async Task<ActionResult<PurchaseItemReadDto>> AddPurchaseItem(int purchaseOrderId, [FromBody] PurchaseItemCreateDto dto)
        {
            var created = await _purchaseOrderService.AddPurchaseItemAsync(purchaseOrderId, dto);
            return CreatedAtAction(nameof(GetById), new { id = purchaseOrderId }, created);
        }

        [HttpPut("items/{id}")]
        public async Task<IActionResult> UpdatePurchaseItem(int id, [FromBody] PurchaseItemUpdateDto dto)
        {
            var updated = await _purchaseOrderService.UpdatePurchaseItemAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("items/{id}")]
        public async Task<IActionResult> RemovePurchaseItem(int id)
        {
            var removed = await _purchaseOrderService.RemovePurchaseItemAsync(id);
            if (!removed) return NotFound();
            return NoContent();
        }

        // ----------------------------
        // Received Stocks
        // ----------------------------
        [HttpPost("received")]
        public async Task<ActionResult<ReceivedStockReadDto>> AddReceivedStock([FromBody] ReceivedStockCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
            var created = await _purchaseOrderService.AddReceivedStockAsync(dto, userId);
            return Ok(created);
        }

        [HttpDelete("received/{id}")]
        public async Task<IActionResult> DeleteReceivedStock(int id)
        {
            var deleted = await _purchaseOrderService.DeleteReceivedStockAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
