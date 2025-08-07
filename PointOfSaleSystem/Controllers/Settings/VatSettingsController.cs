using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSystem.DTOs.Settings;
using PointOfSaleSystem.Interfaces.Settings;

namespace PointOfSaleSystem.Controllers.Settings
{
    [ApiController]
    [Route("api/settings/vat")]
    [Authorize(Roles = "Admin")]
    public class VatSettingsController : ControllerBase
    {
        private readonly IVatSettingService _vatSettingService;

        public VatSettingsController(IVatSettingService vatSettingService)
        {
            _vatSettingService = vatSettingService;
        }

        // GET: api/settings/vat
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VatSettingReadDto>>> GetAll()
        {
            var result = await _vatSettingService.GetAllAsync();
            return Ok(result);
        }

        // GET: api/settings/vat/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<VatSettingReadDto>> GetById(int id)
        {
            var result = await _vatSettingService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // POST: api/settings/vat
        [HttpPost]
        public async Task<ActionResult<VatSettingReadDto>> Create(VatSettingCreateDto dto)
        {
            var result = await _vatSettingService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT: api/settings/vat
        [HttpPut]
        public async Task<ActionResult> Update(VatSettingUpdateDto dto)
        {
            var success = await _vatSettingService.UpdateAsync(dto);
            if (!success) return NotFound();
            return NoContent();
        }

        // PATCH: api/settings/vat/{id}/status?isActive=true
        [HttpPatch("{id}/status")]
        public async Task<ActionResult> SetActive(int id, [FromQuery] bool isActive)
        {
            var success = await _vatSettingService.SetActiveAsync(id, isActive);
            if (!success) return NotFound();
            return NoContent();
        }

    }
}
