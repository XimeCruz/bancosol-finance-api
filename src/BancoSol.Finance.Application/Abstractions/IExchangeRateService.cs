namespace BancoSol.Finance.Application.Abstractions;

public interface IExchangeRateService
{
    Task<decimal> GetUsdToBobRateAsync(CancellationToken cancellationToken);
}
