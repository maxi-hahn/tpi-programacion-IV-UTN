using Application.Dtos.Request;
using Application.Dtos.Responses;
using Application.Interfaces;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Authorization;

namespace Presentation.Controller
{
    [ApiController]
    // Usamos esta ruta para que los hijos hereden la ruta base o la definan ellos
    [Route("api/[controller]")]
    public abstract class UsersController<T> : ControllerBase where T : User
    {
        protected readonly IUserService _service;
        protected readonly IAuthService _authService;

        public UsersController(IUserService service, IAuthService authService)
        {
            _service = service;
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("singin")]
        public async Task<ActionResult<SingInResponse>> SingIn(
            [FromBody] SingInRequest request)
        {
            var response = await _authService.SingIn(request);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<ActionResult<SingUpResponse>> SingUp(
            [FromBody] SingUpRequest request)
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
        public async Task<IActionResult> ResendVerification(
            [FromBody] ResendVerificationRequest request)
        {
            await _authService.ResendVerificationEmail(request.Email);

            return Ok("Correo de verificacion enviado.");
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordRequest request)
        {
            await _authService.ForgotPassword(request.Email);

            return Ok("Se envio el correo de recuperacion.");
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordRequest request)
        {
            await _authService.ResetPassword(request.Token, request.NewPassword);

            return Ok("Contraseña actualizada correctamente.");
        }

        
        [Authorize(Policy = Policies.AdminOSysAdmin)]
        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<T>>> Get()
        {
            var users = await _service.GetAll();
            // devolver solo el tipo específico (Client, Admin, etc.)
            return Ok(users.OfType<T>());
        }

        //para que el usuario pueda actualizar su perfil, sin necesidad de ser admin
        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile(UpdateUserRequest request)
        {
            var result = await _service.UpdateUser(request);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("claims")]
        public IActionResult Claims()
        {
            return Ok(User.Claims.Select(c => new
            {
                c.Type,
                c.Value
            }));
        }

        [Authorize(Policy = Policies.AdminOSysAdmin)]
        [HttpGet("{id}")]
        public virtual async Task<ActionResult<T>> GetById(Guid id)
        {
            var user = await _service.GetById(id);
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost]
        public virtual async Task<ActionResult<T>> Post(T user)
        {
            var created = await _service.Create(user);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, (T)created);
        }


        [Authorize(Policy = Policies.AdminOSysAdmin)]
        [HttpPatch("{id}")]
        public virtual async Task<IActionResult> Patch(Guid id, T user)
        {
            await _service.Update(id, user);

            return NoContent();
        }

        [Authorize(Policy = Policies.AdminOSysAdmin)]
        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(Guid id)
        {
            await _service.Delete(id);

            return NoContent();
        }
    }
}
