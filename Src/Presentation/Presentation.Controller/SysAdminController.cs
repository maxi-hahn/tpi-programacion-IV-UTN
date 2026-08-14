using Application.Constants;
using Application.Dtos.Request;
using Application.Interfaces;
using Application.Templates;
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
        private readonly IEmailService _emailService;
        public SysAdminController(IUserService service, ISysAdminService sysAdminService, IEmailService emailService)
        {
            _sysAdminService = sysAdminService;
            _service = service;
            _emailService = emailService;

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
        [Authorize(Policy = Policies.AdminOSysAdmin)]
        [HttpPatch("ToggleUserStatus")]
        public async Task<IActionResult> ToggleUserStatus([FromQuery] Guid userId)
        {
            var user = await _service.GetById(userId);

            if (user == null)
                return NotFound(new { message = "Usuario no encontrado." });

            // Cambiar estado
            user.IsActive = !user.IsActive;
            await _service.Update(userId, user);

            // Enviar email de notificación
            try
            {
                if (user.IsActive)
                {
                    await _emailService.SendEmailAsync(
                        user.Email,
                        EmailSubjects.userActivated,
                        EmailTemplates.UserActivated(user.Name)
                    );
                }
                else
                {
                    await _emailService.SendEmailAsync(
                        user.Email,
                        EmailSubjects.userDeactivated,
                        EmailTemplates.UserDeactivated(user.Name)
                    );
                }
            }
            catch (Exception ex)
            {
                // Log del error pero no bloquear la operación
                Console.WriteLine($"Error sending email: {ex.Message}");
            }

            return Ok(new
            {
                message = user.IsActive ? "Usuario activado correctamente." : "Usuario desactivado correctamente.",
                isActive = user.IsActive
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

