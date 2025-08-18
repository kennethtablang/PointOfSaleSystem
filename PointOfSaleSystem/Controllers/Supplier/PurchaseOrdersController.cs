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
    public class PurchaseOrdersController : Controller
    {
        private readonly IPurchaseOrderService _poService;

        public PurchaseOrdersController(IPurchaseOrderService poService)
        {
            _poService = poService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _poService.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var po = await _poService.GetByIdAsync(id);
            if (po == null) return NotFound();
            return Ok(po);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PurchaseOrderCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

            var created = await _poService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PurchaseOrderUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != dto.Id) return BadRequest("Id mismatch.");

            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var updated = await _poService.UpdateAsync(dto, userId);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _poService.DeleteAsync(id);
                if (!success) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST api/PurchaseOrders/receive
        [HttpPost("receive")]
        public async Task<IActionResult> Receive([FromBody] ReceiveStockCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            try
            {
                var created = await _poService.ReceiveStockAsync(dto, userId);
                return Ok(created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE api/PurchaseOrders/items/{itemId}
        [HttpDelete("items/{itemId:int}")]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            try
            {
                var ok = await _poService.RemoveItemByIdAsync(itemId);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET api/stockreceivings/pending
        [HttpGet("pending")]
        public async Task<IActionResult> StockReceivings()
        {
            var list = await _poService.GetPendingReceivingsAsync();
            return Ok(list);
        }

        // POST api/stockreceivings/{poId}/post
        [HttpPost("{poId}/post")]
        public async Task<IActionResult> StockReceivings(int poId)
        {
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            await _poService.PostReceivedToInventoryAsync(poId, userId);
            return NoContent();
        }

        // DELETE api/PurchaseOrders/received/{receivedId}
        [HttpDelete("received/{receivedId:int}")]
        public async Task<IActionResult> DeleteReceived(int receivedId)
        {
            try
            {
                var ok = await _poService.DeleteReceivedStockAsync(receivedId);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
