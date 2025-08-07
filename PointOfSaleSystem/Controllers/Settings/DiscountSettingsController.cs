using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSystem.DTOs.Settings;
using PointOfSaleSystem.Interfaces.Settings;

namespace PointOfSaleSystem.Controllers.Settings
{
    [ApiController]
    [Route("api/settings/discounts")]
    [Authorize(Roles = "Admin")]
    public class DiscountSettingsController : ControllerBase
    {
        private readonly IDiscountSettingService _discountSettingService;

        public DiscountSettingsController(IDiscountSettingService discountSettingService)
        {
            _discountSettingService = discountSettingService;
        }

        // GET: api/settings/discounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiscountSettingReadDto>>> GetAll()
        {
            var discounts = await _discountSettingService.GetAllAsync();
            return Ok(discounts);
        }

        // GET: api/settings/discounts/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DiscountSettingReadDto>> GetById(int id)
        {
            var discount = await _discountSettingService.GetByIdAsync(id);
            if (discount == null)
                return NotFound();

            return Ok(discount);
        }

        // POST: api/settings/discounts
        [HttpPost]
        public async Task<ActionResult<DiscountSettingReadDto>> Create([FromBody] DiscountSettingCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _discountSettingService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/settings/discounts/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] DiscountSettingUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _discountSettingService.UpdateAsync(id, dto);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        // PATCH: api/settings/discounts/{id}/deactivate
        [HttpPatch("{id}/deactivate")]
        public async Task<ActionResult> Deactivate(int id)
        {
            var success = await _discountSettingService.DeactivateAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
