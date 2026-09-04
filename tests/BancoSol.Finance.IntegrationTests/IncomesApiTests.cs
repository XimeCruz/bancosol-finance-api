using System.Net;
using System.Net.Http.Json;
using BancoSol.Finance.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BancoSol.Finance.IntegrationTests;

public sealed class IncomesApiTests : IClassFixture<IncomesApiFactory>
{
    private readonly HttpClient _client;

    public IncomesApiTests(IncomesApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_WithValidIncome_ReturnsCreatedAndCanBeRetrieved()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/v1/incomes",
            new
            {
                amount = 5000m,
                description = "Sueldo diciembre",
                receivedDate = "2025-12-01",
                source = "Sueldo",
                currency = "BOB"
            },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var getResponse = await _client.GetAsync(
            response.Headers.Location!,
            TestContext.Current.CancellationToken);

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Post_WithEur_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/v1/incomes",
            new
            {
                amount = 100m,
                description = "Venta",
                receivedDate = "2025-12-01",
                source = "Venta",
                currency = "EUR"
            },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_WithUnknownValidId_ReturnsNotFoundWithClearMessage()
    {
        var id = Guid.NewGuid();
        var response = await _client.GetAsync(
            $"/api/v1/incomes/{id}",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        body.Should().Contain("no está registrado");
        body.Should().Contain(id.ToString());
    }

    [Fact]
    public async Task Get_WithMalformedId_ReturnsBadRequestWithClearMessage()
    {
        var response = await _client.GetAsync(
            "/api/v1/incomes/id-invalido",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().Contain("formato GUID válido");
    }
}


public sealed class IncomesApiFactory : WebApplicationFactory<Program>
{
   protected override void ConfigureWebHost(IWebHostBuilder builder)
{
    builder.UseEnvironment("Testing");

    var connectionString =
        Environment.GetEnvironmentVariable(
            "ConnectionStrings__FinanceTestDatabase")
        ?? throw new InvalidOperationException(
            "No se configuró ConnectionStrings__FinanceTestDatabase.");

    builder.ConfigureServices(services =>
    {
        services.RemoveAll<DbContextOptions<FinanceDbContext>>();

        services.RemoveAll<
            IDbContextOptionsConfiguration<FinanceDbContext>>();

        services.AddDbContext<FinanceDbContext>(options =>
            options.UseNpgsql(connectionString));
    });
}
}
