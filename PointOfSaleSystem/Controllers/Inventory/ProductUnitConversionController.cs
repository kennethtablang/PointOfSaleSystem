using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSystem.DTOs.Inventory;
using PointOfSaleSystem.Interfaces.Inventory;

namespace PointOfSaleSystem.Controllers.Inventory
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles ="Admin,Manager")]
    public class ProductUnitConversionController : ControllerBase
    {
        private readonly IProductUnitConversionService _service;
        private readonly ILogger<ProductUnitConversionController> _logger;

        public ProductUnitConversionController(IProductUnitConversionService service, ILogger<ProductUnitConversionController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: api/ProductUnitConversion
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        // GET: api/ProductUnitConversion/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // GET: api/ProductUnitConversion/by-product/3
        [HttpGet("by-product/{productId:int}")]
        public async Task<IActionResult> GetByProduct(int productId)
        {
            var list = await _service.GetByProductIdAsync(productId);
            return Ok(list);
        }

        // POST: api/ProductUnitConversion
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductUnitConversionCreateDto dto)
        {
            // [ApiController] will handle ModelState validation for DataAnnotations / IValidatableObject.
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Create failed due to business rule violation.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating ProductUnitConversion.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // PUT: api/ProductUnitConversion/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUnitConversionUpdateDto dto)
        {
            // Ensure route id matches body id
            if (id != dto.Id) return BadRequest(new { message = "Id mismatch between route and payload." });

            try
            {
                var updated = await _service.UpdateAsync(dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Update failed: not found.");
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Update failed due to business rule violation.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating ProductUnitConversion.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // DELETE: api/ProductUnitConversion/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Delete failed: not found.");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting ProductUnitConversion.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}
