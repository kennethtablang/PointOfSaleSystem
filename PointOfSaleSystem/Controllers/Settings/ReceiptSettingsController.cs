using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSystem.DTOs.Settings;
using PointOfSaleSystem.Interfaces.Settings;

namespace PointOfSaleSystem.Controllers.Settings
{
    [ApiController]
    [Route("api/settings/receipt")]
    [Authorize(Roles = "Admin")]
    public class ReceiptSettingsController : ControllerBase
    {
        private readonly IReceiptSettingService _receiptSettingService;

        public ReceiptSettingsController(IReceiptSettingService receiptSettingService)
        {
            _receiptSettingService = receiptSettingService;
        }

        // GET: api/settings/receipt
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReceiptSettingReadDto>>> GetAll()
        {
            var settings = await _receiptSettingService.GetAllAsync();
            return Ok(settings);
        }

        // GET: api/settings/receipt/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ReceiptSettingReadDto>> GetById(int id)
        {
            var setting = await _receiptSettingService.GetByIdAsync(id);
            if (setting == null) return NotFound();
            return Ok(setting);
        }

        // POST: api/settings/receipt
        [HttpPost]
        public async Task<ActionResult<ReceiptSettingReadDto>> Create([FromBody] ReceiptSettingCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _receiptSettingService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/settings/receipt/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] ReceiptSettingUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var success = await _receiptSettingService.UpdateAsync(id, dto);
            if (!success) return NotFound();
            return NoContent();
        }

        // PATCH: api/settings/receipt/{id}/status?isActive={bool}
        [HttpPatch("{id}/status")]
        public async Task<ActionResult> SetActive(int id, [FromQuery] bool isActive)
        {
            var success = await _receiptSettingService.SetActiveAsync(id, isActive);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
