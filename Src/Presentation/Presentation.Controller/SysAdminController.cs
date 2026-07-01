using Application.Dtos.Request;
using Application.Interfaces;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Authorization;
using Presentation.Controller;


namespace Presentation.Presentation.Controller
{

    [Authorize(Policy = Policies.SoloSysAdmin)]
    public class SysAdminController : UsersController<SysAdmin>
    {
        private readonly ISysAdminService _sysAdminService;
        public SysAdminController(IUserService service, IAuthService authService, ISysAdminService sysAdminService) : base(service, authService)
        {
            _sysAdminService = sysAdminService;

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
    }
}

