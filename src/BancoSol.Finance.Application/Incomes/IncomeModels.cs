using BancoSol.Finance.Domain.Entities;

namespace BancoSol.Finance.Application.Incomes;

public sealed record CreateIncomeCommand(decimal Amount, string Description, DateOnly ReceivedDate, string Source, string Currency);

/// <summary>Ingreso registrado en el sistema.</summary>
/// <param name="Id">Identificador único del ingreso.</param>
/// <param name="Amount">Monto recibido.</param>
/// <param name="Description">Descripción del ingreso.</param>
/// <param name="ReceivedDate">Fecha de recepción.</param>
/// <param name="Source">Procedencia del dinero.</param>
/// <param name="Currency">Moneda BOB o USD.</param>
/// <param name="CreatedAtUtc">Fecha y hora UTC de registro.</param>
/// <example>
/// { "id": "82ab4f7c-32cd-4e3a-9f49-f73701ce7230", "amount": 5000.00, "description": "Sueldo diciembre", "receivedDate": "2025-12-01", "source": "Sueldo", "currency": "BOB", "createdAtUtc": "2026-09-03T13:40:59Z" }
/// </example>
public sealed record IncomeDto(Guid Id, decimal Amount, string Description, DateOnly ReceivedDate, string Source, string Currency, DateTimeOffset CreatedAtUtc)
{
    public static IncomeDto From(Income value) => new(value.Id, value.Amount, value.Description, value.ReceivedDate, value.Source, value.Currency.ToString(), value.CreatedAtUtc);
}
