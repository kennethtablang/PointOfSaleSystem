using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSystem.DTOs.Settings;
using PointOfSaleSystem.Interfaces.Settings;

namespace PointOfSaleSystem.Controllers.Settings
{
    [ApiController]
    [Route("api/settings/counters")]
    [Authorize(Roles = "Admin")]
    public class CounterSettingController : ControllerBase
    {
        private readonly ICounterService _counterService;
        public CounterSettingController(ICounterService counterService)
        {
            _counterService = counterService;
        }

        // GET: api/settings/counters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CounterReadDto>>> GetAll()
        {
            var counters = await _counterService.GetAllAsync();
            return Ok(counters);
        }

        // GET: api/settings/counters/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CounterReadDto>> GetById(int id)
        {
            var counter = await _counterService.GetByIdAsync(id);
            if (counter == null) return NotFound();
            return Ok(counter);
        }

        // POST: api/settings/counters
        [HttpPost]
        public async Task<ActionResult<CounterReadDto>> Create([FromBody] CounterCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _counterService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/settings/counters/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] CounterUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var success = await _counterService.UpdateAsync(id, dto);
            if (!success) return NotFound();
            return NoContent();
        }

        // PATCH: api/settings/counters/{id}/status?isActive={bool}
        [HttpPatch("{id}/status")]
        public async Task<ActionResult> SetActive(int id, [FromQuery] bool isActive)
        {
            var success = await _counterService.SetActiveAsync(id, isActive);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
