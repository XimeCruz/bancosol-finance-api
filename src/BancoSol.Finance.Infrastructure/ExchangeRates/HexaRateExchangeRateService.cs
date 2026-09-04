using System.Net.Http.Json;
using System.Text.Json.Serialization;
using BancoSol.Finance.Application.Abstractions;
using BancoSol.Finance.Application.Common;

namespace BancoSol.Finance.Infrastructure.ExchangeRates;

public sealed class HexaRateExchangeRateService(HttpClient httpClient) : IExchangeRateService
{
    public async Task<decimal> GetUsdToBobRateAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var response = await httpClient.GetAsync("api/rates/USD/BOB/latest", cancellationToken);
            response.EnsureSuccessStatusCode();
            var payload = await response.Content.ReadFromJsonAsync<HexaRateEnvelope>(cancellationToken);
            if (payload?.Data?.Mid is null or <= 0)
                throw new ExternalServiceException("HexaRate devolvió un tipo de cambio inválido.");
            return payload.Data.Mid.Value;
        }
        catch (ExternalServiceException) { throw; }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            throw new ExternalServiceException("No fue posible obtener el tipo de cambio de HexaRate.", exception);
        }
    }

    private sealed record HexaRateEnvelope([property: JsonPropertyName("data")] HexaRateData? Data);
    private sealed record HexaRateData([property: JsonPropertyName("mid")] decimal? Mid);
}
