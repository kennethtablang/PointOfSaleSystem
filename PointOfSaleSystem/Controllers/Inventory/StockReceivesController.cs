using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSystem.Interfaces.Inventory;
using System.Security.Claims;

namespace PointOfSaleSystem.Controllers.Inventory
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager")]
    public class StockReceivesController : ControllerBase
    {
        private readonly IStockReceiveService _service;

        public StockReceivesController(IStockReceiveService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var rec = await _service.GetByIdAsync(id);
            if (rec == null) return NotFound();
            return Ok(rec);
        }

        [HttpGet("by-po/{poId:int}")]
        public async Task<IActionResult> GetByPurchaseOrder(int poId)
        {
            var list = await _service.GetByPurchaseOrderIdAsync(poId);
            return Ok(list);
        }

        // POST api/stockreceives/from-po/{poId}
        [HttpPost("from-po/{poId:int}")]
        public async Task<IActionResult> CreateFromPo(int poId, [FromQuery] bool allowOverReceive = false)
        {
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            try
            {
                var created = await _service.CreateFromReceivedStocksAsync(poId, userId, allowOverReceive);
                return Ok(created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
