using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Dtos.Request;
using Application.Interfaces;
using Application.Mapper;
using Presentation.Authorization;

namespace Presentation.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _service;

        public ScheduleController(IScheduleService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var schedules = await _service.GetAll();
            return Ok(schedules.Select(s => s.ToScheduleResponse()));
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(Guid id)
        {
            var schedule = await _service.GetById(id);

            return Ok(schedule);
        }

        [Authorize(Policy = Policies.AdminOSysAdmin)]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateScheduleRequest dto)
        {
            var schedule = dto.ToSchedule();

            
            var created = await _service.Create(schedule);
            return Ok(created.ToScheduleResponse());
        }

        [Authorize(Policy = Policies.AdminOSysAdmin)]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(Guid id, [FromBody] UpdateScheduleRequest dto)
        {
            var schedule = dto.ToSchedule();


            await _service.Update(id, schedule);

            return NoContent();
        }

        [Authorize(Policy = Policies.AdminOSysAdmin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.Delete(id);

            return NoContent();
        }
    }
}