using Application.Dtos.Request;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Authorization;

namespace Presentation.Presentation.Controller
{
    [Authorize(Policy = Policies.SoloClient)]
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
        [HttpDelete("{scheduleId}")]
        public async Task<IActionResult> Unsubscribe(Guid scheduleId)
        {
            var result = await _service.Unsubscribe(_userContext.UserId, scheduleId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyInscriptions()
        {
            var result = await _service.GetMyInscriptions(_userContext.UserId);
            return Ok(result);
        }
    }
}
