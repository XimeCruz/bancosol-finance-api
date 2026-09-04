using BancoSol.Finance.Application.Balances;
using Microsoft.AspNetCore.Mvc;

namespace BancoSol.Finance.Api.Controllers;

[ApiController]
[Route("api/v1/balances")]
public sealed class BalancesController(BalanceService service) : ControllerBase
{
    /// <summary>Calcula el balance consolidado de un período.</summary>
    /// <remarks>El rango es inclusivo. Los ingresos se convierten usando el tipo de cambio USD/BOB vigente obtenido desde HexaRate.</remarks>
    /// <param name="from">Fecha inicial inclusiva en formato YYYY-MM-DD. Ejemplo: 2026-08-01.</param>
    /// <param name="to">Fecha final inclusiva en formato YYYY-MM-DD. Ejemplo: 2026-08-31.</param>
    /// <param name="currency">Moneda del resultado: BOB o USD. Ejemplo: BOB.</param>
    /// <param name="cancellationToken">Token para cancelar la solicitud.</param>
    /// <returns>Total consolidado, tasa aplicada y cantidad de ingresos incluidos.</returns>
    /// <response code="200">Balance calculado correctamente.</response>
    /// <response code="400">Rango de fechas o moneda inválidos.</response>
    /// <response code="503">HexaRate no está disponible o devolvió una tasa inválida.</response>
    [HttpGet]
    [EndpointName("GetConsolidatedBalance")]
    [ProducesResponseType<BalanceDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<BalanceDto>> Get([FromQuery] DateOnly from, [FromQuery] DateOnly to, [FromQuery] string currency, CancellationToken cancellationToken) =>
        Ok(await service.GetAsync(from, to, currency, cancellationToken));
}
