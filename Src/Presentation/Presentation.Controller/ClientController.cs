using Application.Interfaces;
using Domain.Entity;
using Domain.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controller;
using System.Security.Claims;

namespace Presentation.Presentation.Controller
{
    public class ClientController : UsersController<Client>
    {
        private readonly IPlanRepository _planRepo;
        private readonly IMercadoPagoService _mercadoPagoService;

        public ClientController(
            IUserService service,
            IAuthService authService,
            IMercadoPagoService mercadoPagoService,
            IPlanRepository planRepo) : base(service, authService)
        {
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

        [Authorize]
        [HttpPost("BuyPlan")]
        public async Task<IActionResult> CreatePayment(Guid planId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var plan = await _planRepo.GetById(planId);
            var initPoint = await _mercadoPagoService.CreatePreference(plan, userId);

            return Ok(new { PaymentUrl = initPoint });
        }
    }
}