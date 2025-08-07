using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSystem.DTOs.Inventory;
using PointOfSaleSystem.Interfaces.Inventory;

namespace PointOfSaleSystem.Controllers.Inventory
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UnitsController : ControllerBase
    {
        private readonly IUnitService _unitService;
        private readonly IMapper _mapper;

        public UnitsController(IUnitService unitService, IMapper mapper)
        {
            _unitService = unitService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UnitViewDto>>> GetAll()
        {
            var units = await _unitService.GetAllAsync();
            return Ok(units);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UnitViewDto>> GetById(int id)
        {
            var unit = await _unitService.GetByIdAsync(id);
            if (unit == null) return NotFound();
            return Ok(unit);
        }

        [HttpPost]
        public async Task<ActionResult> Create(UnitCreateDto dto)
        {
            var created = await _unitService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UnitUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest();
            var exists = await _unitService.GetByIdAsync(id);
            if (exists == null) return NotFound();

            var updated = await _unitService.UpdateAsync(dto);
            return updated ? NoContent() : StatusCode(500);
        }

        [HttpPatch("{id}/deactivate")]
        public async Task<ActionResult> Deactivate(int id)
        {
            var success = await _unitService.DeactivateAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
