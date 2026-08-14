using Application.Dtos.Request;
using Application.Dtos.Responses;
using Application.Exceptions;
using Application.Interfaces;
using Application.Services;
using Domain.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Authorization;
using System.Security.Claims;

namespace Presentation.Presentation.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IPlanRepository _planRepo;
        private readonly IMercadoPagoService _mercadoPagoService;
        private readonly IUserService _service;
        private readonly IClientService _clientService;
        public ClientController(IUserService service, IMercadoPagoService mercadoPagoService, IPlanRepository planRepo, IClientService clientService)
        {
            _service = service;
            _planRepo = planRepo;
            _mercadoPagoService = mercadoPagoService;
            _clientService = clientService;
        }

        [HttpPost("webhook/mercadopago")]
        public async Task<IActionResult> Webhook(
            [FromQuery] string? topic,
            [FromQuery] string? type,
            [FromQuery] string? id)
        {
            var eventType = topic ?? type;

            if (eventType != "payment")
                return Ok();

            if (string.IsNullOrEmpty(id))
                return Ok();

            await _mercadoPagoService.ProcessPayment(id);

            return Ok();
        }
        
        [Authorize(Policy = Policies.SoloClient)]
        [HttpPost("BuyPlan")]
        public async Task<IActionResult> CreatePayment(Guid planId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var plan = await _planRepo.GetById(planId);
            var initPoint = await _mercadoPagoService.CreatePreference(plan, userId);

            return Ok(new { PaymentUrl = initPoint });
        }

        [Authorize(Policy = Policies.SoloClient)]
        [HttpGet("me")]
        public async Task<ActionResult<ClientPlanResponse>> GetMe()
        {
            var result = await _clientService.GetMyPlanStatus();
            return Ok(result);
        }

        [Authorize(Policy = Policies.AdminOSysAdmin)]
        [HttpGet]
        public virtual async Task<ActionResult> Get()
        {
            var users = await _service.GetAll();
            var result = users.Select(u => new UserListResponse
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Rol = u.GetType().Name,
                IsActive = u.IsActive,
            });
            return Ok(result);
        }

        ////para que el usuario pueda actualizar su perfil, sin necesidad de ser admin
        [Authorize]
        [HttpPut("UpdateMe")]
        public async Task<IActionResult> UpdateProfile(UpdateUserRequest request)
        {
            var result = await _service.UpdateUser(request);

            return Ok(result);
        }



        [Authorize(Policy = Policies.AdminOSysAdmin)]
        [HttpGet("{id}")]
        public virtual async Task<ActionResult> GetById(Guid id)
        {
            var user = await _service.GetById(id);
            return Ok(user);
        }

        [Authorize(Policy = Policies.AdminOSysAdmin)]
        [HttpGet("{userId}/plan")]
        public async Task<ActionResult<ClientPlanResponse>> GetUserPlan(Guid userId)
        {
            try
            {
                var result = await _clientService.GetUserPlan(userId);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Policy = Policies.AdminOSysAdmin)]
        [HttpDelete("{userId}/plan")]
        public async Task<ActionResult> RemoveUserPlan(Guid userId)
        {
            try
            {
                await _clientService.RemoveUserPlan(userId);
                return Ok(new { message = "Plan removido correctamente." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}