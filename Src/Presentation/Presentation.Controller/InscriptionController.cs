using Application.Dtos.Request;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Authorization;

namespace Presentation.Presentation.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class InscriptionController : ControllerBase
    {
        private readonly IInscriptionService _service;
        private readonly IUserContext _userContext;

        public InscriptionController(IInscriptionService service, IUserContext userContext)
        {
            _service = service;
            _userContext = userContext;
        }

        [Authorize(Policy = Policies.SoloClient)]
        [HttpPost]
        public async Task<IActionResult> Inscribe([FromBody] InscriptionRequest request)
        {
            var result = await _service.Inscribe(_userContext.UserId, request);
            if (!result.Success)
            {
                return BadRequest(new
                {
                    code = result.code,
                    message = result.ErrorMessage
                });
            }

            return Ok(new
            {
                code = result.code,
                message = "Inscripción exitosa.",
                data = result.Data
            });
        }

        [Authorize(Policy = Policies.SoloClient)]
        [HttpDelete("{scheduleId}")]
        public async Task<IActionResult> Unsubscribe(Guid scheduleId)
        {
            var result = await _service.Unsubscribe(_userContext.UserId, scheduleId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Policy = Policies.SoloClient)]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyInscriptions()
        {
            var result = await _service.GetMyInscriptions(_userContext.UserId);
            return Ok(result);
        }

        // Endpoints para administradores
        [Authorize(Policy = Policies.AdminOSysAdmin)]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserInscriptions(Guid userId)
        {
            var result = await _service.GetMyInscriptions(userId);
            return Ok(result);
        }

        [Authorize(Policy = Policies.AdminOSysAdmin)]
        [HttpDelete("user/{userId}/{scheduleId}")]
        public async Task<IActionResult> UnsubscribeUser(Guid userId, Guid scheduleId)
        {
            var adminUserId = _userContext.UserId;
            var result = await _service.UnsubscribeUser(adminUserId, userId, scheduleId);

            if (!result.Success)
                return BadRequest(new
                {
                    code = result.code,
                    message = result.ErrorMessage
                });

            return Ok(new
            {
                code = result.code,
                message = "Usuario desinscripto correctamente.",
                data = result.Data
            });
        }
    }
}