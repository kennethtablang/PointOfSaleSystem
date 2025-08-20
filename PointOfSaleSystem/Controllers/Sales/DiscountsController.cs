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
    public class DiscountsController : ControllerBase
    {
        private readonly IDiscountService _service;
        private readonly ILogger<DiscountsController> _logger;

        public DiscountsController(IDiscountService service, ILogger<DiscountsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? saleId = null, [FromQuery] int? saleItemId = null)
        {
            var list = await _service.GetAllAsync(saleId, saleItemId);
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DiscountCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            try
            {
                var created = await _service.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (KeyNotFoundException knf) { return NotFound(new { message = knf.Message }); }
            catch (InvalidOperationException inv) { return BadRequest(new { message = inv.Message }); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create discount");
                return StatusCode(500, "Failed to create discount");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] DiscountUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (KeyNotFoundException knf) { return NotFound(new { message = knf.Message }); }
            catch (InvalidOperationException inv) { return BadRequest(new { message = inv.Message }); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update discount {Id}", id);
                return StatusCode(500, "Failed to update discount");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var ok = await _service.DeleteAsync(id);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException inv) { return BadRequest(new { message = inv.Message }); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete discount {Id}", id);
                return StatusCode(500, "Failed to delete discount");
            }
        }
    }
}