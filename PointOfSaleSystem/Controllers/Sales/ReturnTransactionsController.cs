using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSystem.DTOs.Sales;
using PointOfSaleSystem.Enums;
using PointOfSaleSystem.Interfaces.Sales;
using System.Security.Claims;

namespace PointOfSaleSystem.Controllers.Sales
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager")]
    public class ReturnTransactionsController : ControllerBase
    {
        private readonly IReturnTransactionService _service;
        private readonly ILogger<ReturnTransactionsController> _logger;

        public ReturnTransactionsController(IReturnTransactionService service, ILogger<ReturnTransactionsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? saleId = null)
        {
            var list = await _service.GetAllAsync(saleId);
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
        public async Task<IActionResult> Create([FromBody] ReturnTransactionCreateDto dto)
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
                _logger.LogError(ex, "Failed to create return transaction");
                return StatusCode(500, "Failed to create return transaction");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ReturnTransactionUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (InvalidOperationException inv) { return BadRequest(new { message = inv.Message }); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update return transaction {Id}", id);
                return StatusCode(500, "Failed to update return transaction");
            }
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> ChangeStatus(int id, [FromQuery] ReturnStatus status)
        {
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            try
            {
                var updated = await _service.ChangeStatusAsync(id, status, userId);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to change status of return transaction {Id}", id);
                return StatusCode(500, "Failed to change return status");
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
                _logger.LogError(ex, "Failed to delete return transaction {Id}", id);
                return StatusCode(500, "Failed to delete return transaction");
            }
        }
    }
}
