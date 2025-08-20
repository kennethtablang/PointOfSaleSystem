using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSystem.DTOs.Sales;
using PointOfSaleSystem.Interfaces.Sales;
using System.Security.Claims;

namespace PointOfSaleSystem.Controllers.Sales
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager")]
    public class SaleItemsController : ControllerBase
    {
        private readonly ISaleItemService _service;
        private readonly ILogger<SaleItemsController> _logger;

        public SaleItemsController(ISaleItemService service, ILogger<SaleItemsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("sale/{saleId:int}")]
        public async Task<IActionResult> GetBySale(int saleId)
        {
            var items = await _service.GetBySaleIdAsync(saleId);
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost("sale/{saleId:int}")]
        public async Task<IActionResult> Create(int saleId, [FromBody] SaleItemCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            try
            {
                var created = await _service.CreateAsync(saleId, dto, user);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (KeyNotFoundException knf) { return NotFound(new { message = knf.Message }); }
            catch (InvalidOperationException inv) { return BadRequest(new { message = inv.Message }); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create sale item");
                return StatusCode(500, "Failed to create sale item");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] SaleItemCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            try
            {
                var updated = await _service.UpdateAsync(id, dto, user);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (InvalidOperationException inv) { return BadRequest(new { message = inv.Message }); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update sale item {Id}", id);
                return StatusCode(500, "Failed to update sale item");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            try
            {
                var ok = await _service.DeleteAsync(id, user);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException inv) { return BadRequest(new { message = inv.Message }); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete sale item {Id}", id);
                return StatusCode(500, "Failed to delete sale item");
            }
        }
    }
}
