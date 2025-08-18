using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSystem.DTOs.Inventory;
using PointOfSaleSystem.Interfaces.Inventory;

namespace PointOfSaleSystem.Controllers.Inventory
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin, Manager")]
    public class InventoryTransactionController : ControllerBase
    {
        private readonly IInventoryTransactionService _service;
        private readonly ILogger<InventoryTransactionController> _logger;

        public InventoryTransactionController(IInventoryTransactionService service, ILogger<InventoryTransactionController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? productId,
            [FromQuery] Enums.InventoryActionType? actionType,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            var list = await _service.GetAllAsync(productId, actionType, from, to);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var t = await _service.GetByIdAsync(id);
            if (t == null) return NotFound();
            return Ok(t);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InventoryTransactionCreateDto dto)
        {
            string userId = User?.Identity?.Name ?? string.Empty;

            try
            {
                var created = await _service.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (KeyNotFoundException knf)
            {
                return NotFound(knf.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create inventory transaction");
                return StatusCode(500, "Failed to create inventory transaction");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] InventoryTransactionUpdateDto dto)
        {
            try
            {
                var ok = await _service.UpdateAsync(id, dto);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update transaction {id}", id);
                return StatusCode(500, "Failed to update transaction");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var ok = await _service.DeleteAsync(id);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete transaction {id}", id);
                return StatusCode(500, "Failed to delete transaction");
            }
        }

        // GET api/InventoryTransaction/products/{productId}/stock
        [HttpGet("products/{productId}/stock")]
        public async Task<IActionResult> GetProductStock(int productId)
        {
            var stock = await _service.GetProductStockAsync(productId);
            return Ok(stock);
        }
    }
}
