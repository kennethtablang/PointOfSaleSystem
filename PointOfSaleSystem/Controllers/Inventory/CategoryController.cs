using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSystem.DTOs.Inventory;
using PointOfSaleSystem.Interfaces.Inventory;

namespace PointOfSaleSystem.Controllers.Inventory
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryViewDto>>> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories); // Already returns CategoryViewDto
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryViewDto>> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CategoryCreateDto dto)
        {
            var created = await _categoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, CategoryUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest();

            var exists = await _categoryService.GetByIdAsync(id);
            if (exists == null) return NotFound();

            var success = await _categoryService.UpdateAsync(dto);
            return success ? NoContent() : StatusCode(500);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var success = await _categoryService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
