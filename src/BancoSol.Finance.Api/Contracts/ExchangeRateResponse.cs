namespace BancoSol.Finance.Api.Contracts;

/// <summary>Tipo de cambio vigente entre USD y BOB.</summary>
/// <param name="BaseCurrency">Moneda base.</param>
/// <param name="QuoteCurrency">Moneda cotizada.</param>
/// <param name="Rate">Cantidad de BOB equivalente a un USD.</param>
/// <example>{ "baseCurrency": "USD", "quoteCurrency": "BOB", "rate": 12.155 }</example>
public sealed record ExchangeRateResponse(string BaseCurrency, string QuoteCurrency, decimal Rate);
