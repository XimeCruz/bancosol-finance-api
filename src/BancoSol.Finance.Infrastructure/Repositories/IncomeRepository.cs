using BancoSol.Finance.Application.Abstractions;
using BancoSol.Finance.Domain.Entities;
using BancoSol.Finance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BancoSol.Finance.Infrastructure.Repositories;

public sealed class IncomeRepository(FinanceDbContext dbContext) : IIncomeRepository
{
    public async Task AddAsync(Income income, CancellationToken cancellationToken)
    {
        dbContext.Incomes.Add(income);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Income>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.Incomes.AsNoTracking().OrderByDescending(x => x.ReceivedDate).ThenByDescending(x => x.CreatedAtUtc).ToListAsync(cancellationToken);

    public Task<Income?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Incomes.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Income>> GetByPeriodAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken) =>
        await dbContext.Incomes.AsNoTracking().Where(x => x.ReceivedDate >= from && x.ReceivedDate <= to).ToListAsync(cancellationToken);
}
