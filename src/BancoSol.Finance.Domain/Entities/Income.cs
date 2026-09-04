using BancoSol.Finance.Domain.Enums;
using BancoSol.Finance.Domain.Exceptions;

namespace BancoSol.Finance.Domain.Entities;

public sealed class Income
{
    private Income() { }

    public Income(decimal amount, string description, DateOnly receivedDate, string source, Currency currency)
    {
        if (amount <= 0) throw new DomainException("El monto debe ser mayor que cero.");
        if (string.IsNullOrWhiteSpace(description)) throw new DomainException("La descripción es obligatoria.");
        if (description.Trim().Length > 250) throw new DomainException("La descripción admite hasta 250 caracteres.");
        if (string.IsNullOrWhiteSpace(source)) throw new DomainException("La fuente del ingreso es obligatoria.");
        if (source.Trim().Length > 100) throw new DomainException("La fuente admite hasta 100 caracteres.");
        if (!Enum.IsDefined(currency)) throw new DomainException("La moneda debe ser BOB o USD.");

        Id = Guid.NewGuid();
        Amount = amount;
        Description = description.Trim();
        ReceivedDate = receivedDate;
        Source = source.Trim();
        Currency = currency;
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }
    public decimal Amount { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public DateOnly ReceivedDate { get; private set; }
    public string Source { get; private set; } = string.Empty;
    public Currency Currency { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
}
