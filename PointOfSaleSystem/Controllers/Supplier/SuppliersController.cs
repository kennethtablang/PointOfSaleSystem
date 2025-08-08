using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSystem.DTOs.Suppliers;
using PointOfSaleSystem.Interfaces.Supplier;

namespace PointOfSaleSystem.Controllers.Supplier
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierReadDto>>> GetAll()
        {
            var suppliers = await _supplierService.GetAllAsync();
            return Ok(suppliers);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SupplierReadDto>> GetById(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null) return NotFound();
            return Ok(supplier);
        }

        [HttpPost]
        public async Task<ActionResult<SupplierReadDto>> Create(SupplierCreateDto dto)
        {
            var createdSupplier = await _supplierService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdSupplier.Id }, createdSupplier);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, SupplierUpdateDto dto)
        {
            var success = await _supplierService.UpdateAsync(id, dto);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _supplierService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
