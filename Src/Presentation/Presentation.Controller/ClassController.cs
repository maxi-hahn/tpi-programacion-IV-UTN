using Application.Dtos.Request;
using Application.Dtos.Responses;
using Application.Interfaces;
using Application.Mapper;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Authorization;

namespace Presentation.Presentation.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassController : ControllerBase
    {
        private readonly IClassService _service;
        private readonly IEmailService _emailService;

        public ClassController(IClassService service, IEmailService emailService)
        {
            _service = service;
            _emailService = emailService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var classes = await _service.GetAll();

            return Ok(classes.Select(c => c.ToClassResponse()));
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Class>> GetById(Guid id)
        {
            var gymClass = await _service.GetById(id);

            return Ok(gymClass);
        }

        [Authorize(Policy = Policies.AdminOSysAdmin)]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateClassRequest dto)
        {
            var gymClass = dto.ToClass();
            await _service.Create(gymClass);
            return Ok(gymClass.ToClassResponse());
            
        }

        [Authorize(Policy = Policies.AdminOSysAdmin)]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(Guid id, [FromBody] UpdateClassRequest dto)
        {
            var gymClass = new Class
            {
                Name = dto.Name!,
                Max_Users = dto.Max_Users
            };

            await _service.Update(id, gymClass);

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