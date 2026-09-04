using BancoSol.Finance.Application.Abstractions;
using BancoSol.Finance.Application.Common;
using BancoSol.Finance.Application.Incomes;
using FluentAssertions;
using NSubstitute;

namespace BancoSol.Finance.UnitTests;

public sealed class IncomeServiceTests
{
    [Fact]
    public async Task Create_WhenCurrencyIsEur_RejectsRequestAndDoesNotPersist()
    {
        var repository = Substitute.For<IIncomeRepository>();
        var service = new IncomeService(repository);

        var action = () => service.CreateAsync(
            new(
                100m,
                "Venta",
                new DateOnly(2025, 12, 1),
                "Venta",
                "EUR"),
            TestContext.Current.CancellationToken);

        await action.Should()
            .ThrowAsync<RequestValidationException>()
            .WithMessage("*BOB o USD*");

        await repository
            .DidNotReceiveWithAnyArgs()
            .AddAsync(
                default!,
                TestContext.Current.CancellationToken);
    }
}