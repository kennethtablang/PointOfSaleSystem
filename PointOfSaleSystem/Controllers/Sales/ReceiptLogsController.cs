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
    public class ReceiptLogsController : ControllerBase
    {
        private readonly IReceiptLogService _service;
        private readonly ILogger<ReceiptLogsController> _logger;

        public ReceiptLogsController(IReceiptLogService service, ILogger<ReceiptLogsController> logger)
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
        public async Task<IActionResult> Create([FromBody] ReceiptLogCreateDto dto)
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
                _logger.LogError(ex, "Failed to create receipt log");
                return StatusCode(500, "Failed to create receipt log");
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
                _logger.LogError(ex, "Failed to delete receipt log {Id}", id);
                return StatusCode(500, "Failed to delete receipt log");
            }
        }

        [HttpGet("sale/{saleId:int}")]
        public async Task<IActionResult> GetBySale(int saleId)
        {
            var list = await _service.GetBySaleIdAsync(saleId);
            return Ok(list);
        }
    }
}
