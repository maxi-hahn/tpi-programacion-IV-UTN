using Application.Dtos.Request;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Authorization;

namespace Presentation.Presentation.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = Policies.SoloSysAdmin)]
    public class SysAdminController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly ISysAdminService _sysAdminService;
        public SysAdminController(IUserService service, ISysAdminService sysAdminService)
        {
            _sysAdminService = sysAdminService;
            _service = service;
        }

        [Authorize]
        [HttpPost("UpgradeUsersRol")]
        public async Task<ActionResult> UpgradeUsersRol([FromBody] UpgradeUsersRol request)
        {
            var result = await _sysAdminService.UpgradeUsersRol(request);

            return Ok(new
            {
                Message = "Role updated successfully",
                User = result.Email
            });
        }

        [Authorize]
        [HttpDelete("deleteUser")]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            await _service.Delete(id);
            return Ok(new
            {
                Message = "User deleted successfully",
            });

        }
    }
}

