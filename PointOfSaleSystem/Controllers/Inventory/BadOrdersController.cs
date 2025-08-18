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
    public class BadOrdersController : ControllerBase
    {
        private readonly IBadOrderService _badOrderService;

        public BadOrdersController(IBadOrderService badOrderService)
        {
            _badOrderService = badOrderService;
        }

        // GET: api/BadOrders
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _badOrderService.GetAllAsync();
            return Ok(list);
        }

        // GET: api/BadOrders/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var dto = await _badOrderService.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        // POST: api/BadOrders
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BadOrderCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

            try
            {
                var created = await _badOrderService.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/BadOrders/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] BadOrderUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var updated = await _badOrderService.UpdateAsync(id, dto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/BadOrders/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var ok = await _badOrderService.DeleteAsync(id);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
