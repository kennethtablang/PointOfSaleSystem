using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSystem.DTOs.Sales;
using PointOfSaleSystem.Interfaces.Sales;

namespace PointOfSaleSystem.Controllers.Sales
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager")]
    public class ReturnedItemsController : ControllerBase
    {
        private readonly IReturnedItemService _service;
        private readonly ILogger<ReturnedItemsController> _logger;

        public ReturnedItemsController(IReturnedItemService service, ILogger<ReturnedItemsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? returnTransactionId = null)
        {
            var list = await _service.GetAllAsync(returnTransactionId);
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        // POST api/ReturnedItems/{returnTransactionId}
        [HttpPost("{returnTransactionId:int}")]
        public async Task<IActionResult> Create(int returnTransactionId, [FromBody] ReturnedItemCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var created = await _service.CreateAsync(returnTransactionId, dto);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (KeyNotFoundException knf) { return NotFound(new { message = knf.Message }); }
            catch (InvalidOperationException inv) { return BadRequest(new { message = inv.Message }); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create returned item for returnTransaction {ReturnId}", returnTransactionId);
                return StatusCode(500, "Failed to create returned item");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ReturnedItemUpdateDto dto)
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
                _logger.LogError(ex, "Failed to update returned item {Id}", id);
                return StatusCode(500, "Failed to update returned item");
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete returned item {Id}", id);
                return StatusCode(500, "Failed to delete returned item");
            }
        }
    }
}
