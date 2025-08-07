using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSystem.DTOs.Settings;
using PointOfSaleSystem.Interfaces.Settings;
using PointOfSaleSystem.Services.Settings;

namespace PointOfSaleSystem.Controllers.Settings
{
    [ApiController]
    [Route("api/settings/[controller]")]
    [Authorize(Roles = "Admin")]
    public class BusinessProfileController : ControllerBase
    {
        private readonly IBusinessProfileService _businessProfileService;

        public BusinessProfileController(IBusinessProfileService businessProfileService)
        {
            _businessProfileService = businessProfileService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var profile = await _businessProfileService.GetAsync();
            return profile == null ? NotFound() : Ok(profile);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BusinessProfileCreateDto dto)
        {
            var result = await _businessProfileService.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BusinessProfileUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest();
            return await _businessProfileService.UpdateAsync(dto)
                ? Ok(new { message = "Updated successfully." })
                : NotFound(new { message = "Profile not found." });
        }

    }
}
