using BancoSol.Finance.Application.Balances;
using BancoSol.Finance.Domain.Entities;
using BancoSol.Finance.Domain.Enums;
using FluentAssertions;

namespace BancoSol.Finance.UnitTests;

public sealed class BalanceCalculatorTests
{
    private readonly BalanceCalculator _calculator = new();

    [Fact]
    public void Calculate_WhenTargetIsBob_ConvertsUsdAndSumsAllIncomes()
    {
        var incomes = new[] { Income(3000m, Currency.BOB), Income(100m, Currency.USD), Income(2000m, Currency.BOB) };
        _calculator.Calculate(incomes, Currency.BOB, 6.92m).Should().Be(5692m);
    }

    [Fact]
    public void Calculate_WhenTargetIsUsd_ConvertsBobAndSumsAllIncomes()
    {
        var incomes = new[] { Income(6920m, Currency.BOB), Income(200m, Currency.USD) };
        _calculator.Calculate(incomes, Currency.USD, 6.92m).Should().Be(1200m);
    }

    [Fact]
    public void Calculate_WhenDivisionProducesFractions_RoundsMoneyToTwoDecimals()
    {
        var incomes = new[] { Income(100m, Currency.BOB) };
        _calculator.Calculate(incomes, Currency.USD, 6.92m).Should().Be(14.45m);
    }

    private static Income Income(decimal amount, Currency currency) => new(amount, "Prueba", new DateOnly(2025, 12, 1), "Prueba", currency);
}
