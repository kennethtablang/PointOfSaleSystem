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
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _service;
        private readonly ILogger<SalesController> _logger;

        public SalesController(ISaleService service, ILogger<SalesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null, [FromQuery] string? cashierUserId = null, [FromQuery] string? invoiceNumber = null)
        {
            var list = await _service.GetAllAsync(from, to, cashierUserId, invoiceNumber);
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpGet("invoice/{invoiceNumber}")]
        public async Task<IActionResult> GetByInvoice(string invoiceNumber)
        {
            var dto = await _service.GetByInvoiceNumberAsync(invoiceNumber);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaleCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            try
            {
                var created = await _service.CreateSaleAsync(dto, userId);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (InvalidOperationException inv) { return BadRequest(new { message = inv.Message }); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create sale");
                return StatusCode(500, "Failed to create sale");
            }
        }

        [HttpPost("{saleId:int}/void")]
        public async Task<IActionResult> Void(int saleId, [FromBody] string reason, [FromQuery] bool isSystemVoid = false, [FromQuery] string? approvalUserId = null)
        {
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            try
            {
                var vtDto = await _service.VoidSaleAsync(saleId, userId, reason, isSystemVoid, approvalUserId);
                return Ok(vtDto);
            }
            catch (KeyNotFoundException knf) { return NotFound(new { message = knf.Message }); }
            catch (InvalidOperationException inv) { return BadRequest(new { message = inv.Message }); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to void sale {SaleId}", saleId);
                return StatusCode(500, "Failed to void sale");
            }
        }

        [HttpPost("{saleId:int}/payments")]
        public async Task<IActionResult> AddPayment(int saleId, [FromBody] PaymentCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            try
            {
                var created = await _service.AddPaymentAsync(saleId, dto, userId);
                return CreatedAtAction(nameof(Get), new { id = saleId }, created);
            }
            catch (KeyNotFoundException knf) { return NotFound(new { message = knf.Message }); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add payment to sale {SaleId}", saleId);
                return StatusCode(500, "Failed to add payment");
            }
        }

        [HttpPost("{saleId:int}/full-refund")]
        public async Task<IActionResult> FullRefund(int saleId, [FromQuery] RefundMethod refundMethod = RefundMethod.Cash)
        {
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            try
            {
                var updated = await _service.FullRefundAsync(saleId, userId, refundMethod);
                return Ok(updated);
            }
            catch (KeyNotFoundException knf) { return NotFound(new { message = knf.Message }); }
            catch (InvalidOperationException inv) { return BadRequest(new { message = inv.Message }); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to full-refund sale {SaleId}", saleId);
                return StatusCode(500, "Failed to full refund sale");
            }
        }

        [HttpPut("{saleId:int}")]
        public async Task<IActionResult> UpdateSale(int saleId, [FromBody] SaleCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var updated = await _service.UpdateSaleAsync(saleId, dto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update sale {SaleId}", saleId);
                return StatusCode(500, "Failed to update sale");
            }
        }

        [HttpGet("{saleId:int}/audit")]
        public async Task<IActionResult> GetAudit(int saleId)
        {
            var trails = await _service.GetAuditTrailsAsync(saleId);
            return Ok(trails);
        }
    }
}
