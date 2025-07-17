using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSystem.DTOs.Inventory.Product;
using PointOfSaleSystem.Interfaces.Inventory;

namespace PointOfSaleSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductViewDto>>> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products); // Already DTO-mapped by the service
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductViewDto>> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult> Create(ProductCreateDto dto)
        {
            var created = await _productService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, ProductUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest();

            var exists = await _productService.GetByIdAsync(id);
            if (exists == null) return NotFound();

            var success = await _productService.UpdateAsync(dto);
            return success ? NoContent() : StatusCode(500);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var success = await _productService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
