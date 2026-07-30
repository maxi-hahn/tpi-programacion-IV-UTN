using Application.Dtos.Request;
using Application.Dtos.Responses;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Presentation.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("singin")]
        public async Task<ActionResult<AuthResponse>> SingIn([FromBody] SingInRequest request)
        {
            var response = await _authService.SingIn(request);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<ActionResult<SingUpResponse>> SingUp([FromBody] SingUpRequest request)
        {
            var response = await _authService.SingUp(request);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            await _authService.VerifyEmail(token);
            return Ok("Email verificado correctamente.");
        }

        [AllowAnonymous]
        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationRequest request)
        {
            await _authService.ResendVerificationEmail(request.Email);
            return Ok("Correo de verificacion enviado.");
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            await _authService.ForgotPassword(request.Email);
            return Ok("Se envio el correo de recuperacion.");
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            await _authService.ResetPassword(request.Token, request.NewPassword);
            return Ok("Contraseña actualizada correctamente.");
        }
    }
}