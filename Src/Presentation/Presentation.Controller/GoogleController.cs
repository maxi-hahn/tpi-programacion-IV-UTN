using Application.Application.Interfaces;
using Application.Dtos.Request;
using Application.Dtos.Responses;
using Application.Interfaces;
using Domain.Entity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/google")]
public class GoogleController : ControllerBase
{
    private readonly IGoogleCalendarService _calendarService;
    private readonly IScheduleService _scheduleService;
    private readonly IInscriptionService _inscriptionService;
    private readonly IClassService _classService;

    public GoogleController(
        IGoogleCalendarService calendarService,
        IScheduleService scheduleService,
        IInscriptionService inscriptionService,
        IClassService classService)
    {
        _calendarService = calendarService;
        _scheduleService = scheduleService;
        _inscriptionService = inscriptionService;
        _classService = classService;
    }

    // login and redirect to google auth, then callback to sync calendar
    [HttpGet("login/{inscriptionId}")]
    public IActionResult Login(Guid inscriptionId)
    {
        return Challenge(
            new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(Callback), new { inscriptionId })
            },
            GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback(Guid inscriptionId)
    {
        try
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded)
                return BadRequest(result.Failure?.ToString());

            var accessToken = result.Properties?.GetTokenValue("access_token");

            if (string.IsNullOrEmpty(accessToken))
                return BadRequest("No access token");

            // Get inscription
            var inscription = await _inscriptionService.GetById(inscriptionId);

            if (inscription == null)
                return NotFound("Inscripción no encontrada.");

            // Get class
            var clase = await _classService.GetById(inscription.ClassId);

            if (clase == null)
                return NotFound("Clase no encontrada.");

            // Get schedules
            var schedules = await _scheduleService.GetByClassId(inscription.ClassId);

            var requests = schedules
                .Where(s => s.IsActive)
                .Select(s => new ClassScheduleRequest
                {
                    Day = s.DayOfWeek,
                    StartTime = s.StartTime.ToTimeSpan(),
                    EndTime = s.EndTime.ToTimeSpan()
                })
                .ToList();
            var entity = new Inscription
            {
                Id = inscription.Id,
                UserId = inscription.UserId,
                ClassId = inscription.ClassId,
                InscriptionDate = inscription.InscriptionDate,
                IsActive = inscription.IsActive
            };
            // sync calendar
            await _calendarService.SyncInscriptionAsync(
                accessToken,
                entity,
                clase.Name,
                requests);

            return Ok("Synced");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.ToString());
        }
    }
}