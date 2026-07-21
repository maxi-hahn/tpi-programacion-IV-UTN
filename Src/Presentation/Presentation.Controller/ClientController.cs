using Application.Dtos.Request;
using Application.Interfaces;
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
        public ClientController(IUserService service, IMercadoPagoService mercadoPagoService, IPlanRepository planRepo)
        {
            _service = service;
            _planRepo = planRepo;
            _mercadoPagoService = mercadoPagoService;
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

        [Authorize(Policy = Policies.AdminOSysAdmin)]
        [HttpGet]
        public virtual async Task<ActionResult> Get()
        {
            var users = await _service.GetAll();
            // devolver solo el tipo específico (Client, Admin, etc.)
            return Ok(users);
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
    }
}