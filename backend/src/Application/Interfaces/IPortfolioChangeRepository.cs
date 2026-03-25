using PortfolioTracker.Domain.Entities;

namespace PortfolioTracker.Application.Interfaces;

public interface IPortfolioChangeRepository
{
    Task<IEnumerable<PortfolioChange>> GetRecentAsync(int count = 50, CancellationToken cancellationToken = default);
    Task<IEnumerable<PortfolioChange>> GetByPortfolioIdAsync(Guid portfolioId, CancellationToken cancellationToken = default);
    Task AddAsync(PortfolioChange change, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
