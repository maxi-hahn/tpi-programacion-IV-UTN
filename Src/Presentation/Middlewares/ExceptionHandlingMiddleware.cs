using Application.Exceptions;
using System.Text.Json;
namespace Presentation.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            ValidationException ex =>
                (StatusCodes.Status400BadRequest, ex.Message),

            NotFoundException ex =>
                (StatusCodes.Status404NotFound, ex.Message),

            ConflictException ex =>
                (StatusCodes.Status409Conflict, ex.Message),

            UnauthorizedException ex =>
                (StatusCodes.Status401Unauthorized, ex.Message),

            ForbiddenException ex =>
                (StatusCodes.Status403Forbidden, ex.Message),

            BadRequestException ex =>
                (StatusCodes.Status400BadRequest, ex.Message),

            DataBaseException ex =>
                (StatusCodes.Status500InternalServerError,
                 "Ocurrio un error en la base de datos"),

            _ =>
                (StatusCodes.Status500InternalServerError,
                 exception.Message)
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new
        {
            status = statusCode,
            error = message
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}