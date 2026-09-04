using BancoSol.Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BancoSol.Finance.Infrastructure.Persistence;

public sealed class FinanceDbContext(DbContextOptions<FinanceDbContext> options) : DbContext(options)
{
    public DbSet<Income> Incomes => Set<Income>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var income = modelBuilder.Entity<Income>();
        income.ToTable("incomes");
        income.HasKey(x => x.Id);
        income.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();
        income.Property(x => x.Description).HasMaxLength(250).IsRequired();
        income.Property(x => x.Source).HasMaxLength(100).IsRequired();
        income.Property(x => x.Currency).HasConversion<string>().HasMaxLength(3).IsRequired();
        income.Property(x => x.ReceivedDate).IsRequired();
        income.Property(x => x.CreatedAtUtc).IsRequired();
        income.HasIndex(x => x.ReceivedDate);
    }
}
