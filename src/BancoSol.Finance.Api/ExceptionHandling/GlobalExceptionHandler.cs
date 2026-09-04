using BancoSol.Finance.Application.Common;
using BancoSol.Finance.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BancoSol.Finance.Api.ExceptionHandling;

public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetails, ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            RequestValidationException or DomainException => (StatusCodes.Status400BadRequest, "Solicitud inválida"),
            NotFoundException => (StatusCodes.Status404NotFound, "Ingreso no registrado"),
            ExternalServiceException => (StatusCodes.Status503ServiceUnavailable, "Servicio externo no disponible"),
            _ => (StatusCodes.Status500InternalServerError, "Error interno")
        };
        if (status == 500) logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", context.TraceIdentifier);
        var detail = status == 500 ? "Ocurrió un error inesperado." : exception.Message;
        context.Response.StatusCode = status;
        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = new ProblemDetails { Status = status, Title = title, Detail = detail, Instance = context.Request.Path }
        });
    }
}
