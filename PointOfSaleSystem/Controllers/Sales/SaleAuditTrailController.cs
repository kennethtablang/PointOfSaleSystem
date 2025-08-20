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
    public class SaleAuditTrailController : ControllerBase
    {
        private readonly ISaleAuditTrailService _service;
        private readonly ILogger<SaleAuditTrailController> _logger;

        public SaleAuditTrailController(ISaleAuditTrailService service, ILogger<SaleAuditTrailController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("{saleId:int}")]
        public async Task<IActionResult> GetBySale(int saleId)
        {
            var list = await _service.GetBySaleIdAsync(saleId);
            return Ok(list);
        }

        [HttpGet("query")]
        public async Task<IActionResult> Query([FromQuery] int? saleId = null, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var list = await _service.QueryAsync(saleId, from, to);
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaleAuditTrailCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

            try
            {
                var created = await _service.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(GetBySale), new { saleId = created.SaleId }, created);
            }
            catch (KeyNotFoundException knf) { return NotFound(new { message = knf.Message }); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create audit trail");
                return StatusCode(500, "Failed to create audit trail");
            }
        }
    }
}
