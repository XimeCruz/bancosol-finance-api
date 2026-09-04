using BancoSol.Finance.Application.Abstractions;
using BancoSol.Finance.Application.Common;
using BancoSol.Finance.Domain.Enums;

namespace BancoSol.Finance.Application.Balances;

/// <summary>Balance consolidado de los ingresos de un período.</summary>
/// <param name="From">Inicio inclusivo del período.</param>
/// <param name="To">Fin inclusivo del período.</param>
/// <param name="Currency">Moneda del total.</param>
/// <param name="Total">Total consolidado y redondeado a dos decimales.</param>
/// <param name="UsdToBobRate">Tipo de cambio USD/BOB aplicado.</param>
/// <param name="IncomeCount">Cantidad de ingresos incluidos.</param>
/// <example>
/// { "from": "2026-08-01", "to": "2026-08-31", "currency": "BOB", "total": 6215.50, "usdToBobRate": 12.155, "incomeCount": 3 }
/// </example>
public sealed record BalanceDto(DateOnly From, DateOnly To, string Currency, decimal Total, decimal UsdToBobRate, int IncomeCount);

public sealed class BalanceService(IIncomeRepository repository, IExchangeRateService exchangeRates, BalanceCalculator calculator)
{
    public async Task<BalanceDto> GetAsync(DateOnly from, DateOnly to, string currencyCode, CancellationToken cancellationToken)
    {
        if (from > to) throw new RequestValidationException("La fecha inicial no puede ser posterior a la fecha final.");
        if (!Enum.TryParse<Currency>(currencyCode, true, out var currency) || !Enum.IsDefined(currency))
            throw new RequestValidationException("La moneda debe ser BOB o USD.");

        var incomes = await repository.GetByPeriodAsync(from, to, cancellationToken);
        var rate = await exchangeRates.GetUsdToBobRateAsync(cancellationToken);
        return new BalanceDto(from, to, currency.ToString(), calculator.Calculate(incomes, currency, rate), rate, incomes.Count);
    }
}
