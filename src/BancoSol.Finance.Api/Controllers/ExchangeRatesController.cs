using BancoSol.Finance.Application.Abstractions;
using BancoSol.Finance.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace BancoSol.Finance.Api.Controllers;

[ApiController]
[Route("api/v1/exchange-rates")]
public sealed class ExchangeRatesController(IExchangeRateService service) : ControllerBase
{
    /// <summary>Consulta el tipo de cambio vigente de USD a BOB.</summary>
    /// <remarks>Obtiene la tasa actual desde el proveedor externo HexaRate.</remarks>
    /// <param name="cancellationToken">Token para cancelar la solicitud.</param>
    /// <returns>Moneda base, moneda cotizada y tasa vigente.</returns>
    /// <response code="200">Tipo de cambio obtenido correctamente.</response>
    /// <response code="503">HexaRate no está disponible o devolvió una respuesta inválida.</response>
    [HttpGet("USD/BOB")]
    [EndpointName("GetUsdToBobExchangeRate")]
    [ProducesResponseType<ExchangeRateResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<ExchangeRateResponse>> Get(CancellationToken cancellationToken)
    {
        var rate = await service.GetUsdToBobRateAsync(cancellationToken);
        return Ok(new ExchangeRateResponse("USD", "BOB", rate));
    }
}
