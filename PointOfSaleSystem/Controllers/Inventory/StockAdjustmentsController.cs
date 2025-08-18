using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSystem.DTOs.Inventory;
using PointOfSaleSystem.Interfaces.Inventory;
using System.Security.Claims;

namespace PointOfSaleSystem.Controllers.Inventory
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager")]
    public class StockAdjustmentsController : ControllerBase
    {
        private readonly IStockAdjustmentService _service;

        public StockAdjustmentsController(IStockAdjustmentService service)
        {
            _service = service;
        }

        // GET: api/StockAdjustments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockAdjustmentListDto>>> GetAll()
        {
            var adjustments = await _service.GetAllAsync();
            return Ok(adjustments);
        }

        // GET: api/StockAdjustments/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<StockAdjustmentReadDto>> GetById(int id)
        {
            var adjustment = await _service.GetByIdAsync(id);
            if (adjustment == null) return NotFound();

            return Ok(adjustment);
        }

        // POST: api/StockAdjustments
        [HttpPost]
        public async Task<ActionResult<StockAdjustmentReadDto>> Create(StockAdjustmentCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var created = await _service.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/StockAdjustments/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<StockAdjustmentReadDto>> Update(int id, StockAdjustmentUpdateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var updated = await _service.UpdateAsync(id, dto, userId);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

        // DELETE: api/StockAdjustments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}
