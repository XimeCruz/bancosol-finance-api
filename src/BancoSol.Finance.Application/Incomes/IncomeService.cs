using BancoSol.Finance.Application.Abstractions;
using BancoSol.Finance.Application.Common;
using BancoSol.Finance.Domain.Entities;
using BancoSol.Finance.Domain.Enums;

namespace BancoSol.Finance.Application.Incomes;

public sealed class IncomeService(IIncomeRepository repository)
{
    public async Task<IncomeDto> CreateAsync(CreateIncomeCommand command, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<Currency>(command.Currency, true, out var currency) || !Enum.IsDefined(currency))
            throw new RequestValidationException("La moneda debe ser BOB o USD.");

        var income = new Income(command.Amount, command.Description, command.ReceivedDate, command.Source, currency);
        await repository.AddAsync(income, cancellationToken);
        return IncomeDto.From(income);
    }

    public async Task<IReadOnlyList<IncomeDto>> GetAllAsync(CancellationToken cancellationToken) =>
        (await repository.GetAllAsync(cancellationToken)).Select(IncomeDto.From).ToArray();

    public async Task<IncomeDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var income = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"El ingreso con ID '{id}' no está registrado.");
        return IncomeDto.From(income);
    }
}
