using BancoSol.Finance.Application.Common;
using BancoSol.Finance.Domain.Entities;
using BancoSol.Finance.Domain.Enums;

namespace BancoSol.Finance.Application.Balances;

public sealed class BalanceCalculator
{
    public decimal Calculate(IEnumerable<Income> incomes, Currency target, decimal usdToBobRate)
    {
        if (usdToBobRate <= 0) throw new RequestValidationException("El tipo de cambio debe ser mayor que cero.");
        return decimal.Round(incomes.Sum(x => Convert(x.Amount, x.Currency, target, usdToBobRate)), 2, MidpointRounding.AwayFromZero);
    }

    private static decimal Convert(decimal amount, Currency source, Currency target, decimal rate) => (source, target) switch
    {
        _ when source == target => amount,
        (Currency.USD, Currency.BOB) => amount * rate,
        (Currency.BOB, Currency.USD) => amount / rate,
        _ => throw new RequestValidationException("Conversión de moneda no soportada.")
    };
}
