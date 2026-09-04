using BancoSol.Finance.Api.ExceptionHandling;
using BancoSol.Finance.Application.Balances;
using BancoSol.Finance.Application.Incomes;
using BancoSol.Finance.Infrastructure;
using BancoSol.Finance.Infrastructure.Persistence;
using Microsoft.AspNetCore.HttpOverrides;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails(options => options.CustomizeProblemDetails = context => context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddScoped<IncomeService>();
builder.Services.AddScoped<BalanceService>();
builder.Services.AddSingleton<BalanceCalculator>();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks().AddDbContextCheck<FinanceDbContext>();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

app.UseForwardedHeaders();
app.UseExceptionHandler();
// app.UseHttpsRedirection();
app.MapOpenApi();
app.MapScalarApiReference("/swagger", options => options.WithTitle("BancoSol Finance API"));
app.MapScalarApiReference("/api-docs", options => options.WithTitle("BancoSol Finance API"));
app.MapControllers();
app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
    await scope.ServiceProvider.GetRequiredService<FinanceDbContext>().Database.EnsureCreatedAsync();

await app.RunAsync();

public partial class Program;
