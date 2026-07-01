using Application.Interfaces;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Authorization;

namespace Presentation.Presentation.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = Policies.AdminOSysAdmin)]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IAdminService _adminService;

        public AdminController(IUserService service, IAdminService adminService)
        {
            _service = service;
            _adminService = adminService;
        }
        //-------------------- Client Inscriptions-----------------------------

        [HttpGet("clientInscriptions/{clientId}")]
        public async Task<ActionResult> GetClientInscriptions(Guid clientId)
        {
            var result = await _adminService.GetClientInscriptions(clientId);
            return Ok(result);
        }

        [Authorize(Policy = Policies.AdminOSysAdmin)]
        [HttpPatch("UpdateUser/{id}")]
        public virtual async Task<IActionResult> Patch(Guid id, User user)
        {
            await _service.Update(id, user);

            return NoContent();
        }
    }
}