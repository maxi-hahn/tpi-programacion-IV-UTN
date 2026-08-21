using Application.Dtos.Request;
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

        public ClassController(IClassService service, IEmailService emailService)
        {
            _service = service;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            try
            {
                Guid? userId = null;

                if (User.Identity?.IsAuthenticated == true)
                {
                    userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
                }

                var classes = await _service.GetAll();

                var response = classes.Select(c => c.ToClassResponse(userId));
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ClassController.Get: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(Guid id)
        {
            var gymClass = await _service.GetById(id);

            return Ok(gymClass?.ToClassResponse());
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
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateClassStatusRequest dto)
        {
            await _service.UpdateStatus(id, dto.IsActive);

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