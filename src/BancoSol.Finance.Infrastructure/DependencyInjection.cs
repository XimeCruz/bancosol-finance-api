using BancoSol.Finance.Application.Abstractions;
using BancoSol.Finance.Infrastructure.ExchangeRates;
using BancoSol.Finance.Infrastructure.Persistence;
using BancoSol.Finance.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BancoSol.Finance.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<FinanceDbContext>(options =>
    options.UseNpgsql(
        configuration.GetConnectionString("FinanceDatabase")));
        services.AddScoped<IIncomeRepository, IncomeRepository>();
        services.AddHttpClient<IExchangeRateService, HexaRateExchangeRateService>(client =>
        {
            client.BaseAddress = new Uri(configuration["HexaRate:BaseUrl"] ?? "https://hexarate.paikama.co/");
            client.Timeout = TimeSpan.FromSeconds(10);
        }).AddStandardResilienceHandler(options =>
        {
            options.Retry.MaxRetryAttempts = 3;
            options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(15);
        });
        return services;
    }
}
