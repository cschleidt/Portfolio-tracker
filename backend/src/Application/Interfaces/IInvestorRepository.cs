using PortfolioTracker.Domain.Entities;

namespace PortfolioTracker.Application.Interfaces;

public interface IInvestorRepository
{
    Task<IEnumerable<Investor>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Investor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Investor?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default);
    Task AddAsync(Investor investor, CancellationToken cancellationToken = default);
    Task UpdateAsync(Investor investor, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
