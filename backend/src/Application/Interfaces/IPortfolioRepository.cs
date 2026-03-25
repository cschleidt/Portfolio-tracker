using PortfolioTracker.Domain.Entities;

namespace PortfolioTracker.Application.Interfaces;

public interface IPortfolioRepository
{
    Task<Portfolio?> GetByInvestorIdAsync(Guid investorId, CancellationToken cancellationToken = default);
    Task<Portfolio?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Portfolio portfolio, CancellationToken cancellationToken = default);
    Task UpdateAsync(Portfolio portfolio, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
