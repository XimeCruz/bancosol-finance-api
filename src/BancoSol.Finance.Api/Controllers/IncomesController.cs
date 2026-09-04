using BancoSol.Finance.Api.Contracts;
using BancoSol.Finance.Application.Common;
using BancoSol.Finance.Application.Incomes;
using Microsoft.AspNetCore.Mvc;

namespace BancoSol.Finance.Api.Controllers;

[ApiController]
[Route("api/v1/incomes")]
[Produces("application/json")]
public sealed class IncomesController(IncomeService service) : ControllerBase
{
    /// <summary>Registra un nuevo ingreso.</summary>
    /// <remarks>El monto debe ser positivo y la moneda debe ser BOB o USD. El encabezado Location apunta al recurso creado.</remarks>
    /// <param name="request">Monto, descripción, fecha, procedencia y moneda del ingreso.</param>
    /// <param name="cancellationToken">Token para cancelar la solicitud.</param>
    /// <returns>El ingreso creado con su identificador único.</returns>
    /// <response code="201">Ingreso registrado correctamente.</response>
    /// <response code="400">Datos inválidos, por ejemplo monto no positivo o moneda distinta de BOB/USD.</response>
    [HttpPost]
    [EndpointName("CreateIncome")]
    [ProducesResponseType<IncomeDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IncomeDto>> Create(CreateIncomeRequest request, CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(new(request.Amount, request.Description, request.ReceivedDate, request.Source, request.Currency), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Consulta el historial completo de ingresos.</summary>
    /// <remarks>Devuelve los registros ordenados desde el ingreso más reciente al más antiguo.</remarks>
    /// <param name="cancellationToken">Token para cancelar la solicitud.</param>
    /// <returns>Todos los ingresos registrados. La lista puede estar vacía.</returns>
    /// <response code="200">Historial obtenido correctamente.</response>
    [HttpGet]
    [EndpointName("GetAllIncomes")]
    [ProducesResponseType<IReadOnlyList<IncomeDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<IncomeDto>>> GetAll(CancellationToken cancellationToken) => Ok(await service.GetAllAsync(cancellationToken));

    /// <summary>Consulta un ingreso por su identificador.</summary>
    /// <param name="id">GUID del ingreso. Ejemplo: 82ab4f7c-32cd-4e3a-9f49-f73701ce7230.</param>
    /// <param name="cancellationToken">Token para cancelar la solicitud.</param>
    /// <returns>El ingreso solicitado.</returns>
    /// <response code="200">Ingreso encontrado.</response>
    /// <response code="400">El identificador no tiene formato GUID.</response>
    /// <response code="404">El GUID es válido, pero el ingreso no está registrado.</response>
    [HttpGet("{id}")]
    [EndpointName("GetIncomeById")]
    [ProducesResponseType<IncomeDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IncomeDto>> GetById(string id, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(id, out var incomeId))
            throw new RequestValidationException($"El identificador '{id}' no tiene un formato GUID válido.");

        return Ok(await service.GetByIdAsync(incomeId, cancellationToken));
    }
}
