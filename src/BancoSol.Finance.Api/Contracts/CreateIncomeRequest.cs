using System.ComponentModel.DataAnnotations;

namespace BancoSol.Finance.Api.Contracts;

/// <summary>Datos necesarios para registrar un ingreso.</summary>
/// <example>
/// { "amount": 5000.00, "description": "Sueldo diciembre", "receivedDate": "2025-12-01", "source": "Sueldo", "currency": "BOB" }
/// </example>
public sealed class CreateIncomeRequest
{
    /// <summary>Monto monetario recibido. Debe ser mayor que cero.</summary>
    /// <example>5000.00</example>
    [Range(typeof(decimal), "0.01", "9999999999999999",
        ParseLimitsInInvariantCulture = true,
        ConvertValueInInvariantCulture = true)]
    public decimal Amount { get; init; }

    /// <summary>Descripción breve del ingreso.</summary>
    /// <example>Sueldo diciembre</example>
    [Required]
    [StringLength(250)]
    public required string Description { get; init; }

    /// <summary>Fecha en la que se recibió el ingreso, en formato YYYY-MM-DD.</summary>
    /// <example>2025-12-01</example>
    public DateOnly ReceivedDate { get; init; }

    /// <summary>Procedencia del dinero, por ejemplo sueldo, freelance o venta.</summary>
    /// <example>Sueldo</example>
    [Required]
    [StringLength(100)]
    public required string Source { get; init; }

    /// <summary>Código de moneda. Solo se admiten BOB y USD.</summary>
    /// <example>BOB</example>
    [Required]
    public required string Currency { get; init; }
}
