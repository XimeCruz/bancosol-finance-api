using BancoSol.Finance.Domain.Entities;

namespace BancoSol.Finance.Application.Abstractions;

public interface IIncomeRepository
{
    Task AddAsync(Income income, CancellationToken cancellationToken);
    Task<IReadOnlyList<Income>> GetAllAsync(CancellationToken cancellationToken);
    Task<Income?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Income>> GetByPeriodAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken);
}
