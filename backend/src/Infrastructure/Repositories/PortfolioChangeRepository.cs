using Microsoft.EntityFrameworkCore;
using PortfolioTracker.Application.Interfaces;
using PortfolioTracker.Domain.Entities;
using PortfolioTracker.Infrastructure.Persistence;

namespace PortfolioTracker.Infrastructure.Repositories;

public class PortfolioChangeRepository : IPortfolioChangeRepository
{
    private readonly AppDbContext _context;

    public PortfolioChangeRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<PortfolioChange>> GetRecentAsync(int count = 50, CancellationToken cancellationToken = default)
        => await _context.PortfolioChanges
            .AsNoTracking()
            .OrderByDescending(c => c.DetectedAt)
            .Take(count)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<PortfolioChange>> GetByPortfolioIdAsync(Guid portfolioId, CancellationToken cancellationToken = default)
        => await _context.PortfolioChanges
            .AsNoTracking()
            .Where(c => c.PortfolioId == portfolioId)
            .OrderByDescending(c => c.DetectedAt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(PortfolioChange change, CancellationToken cancellationToken = default)
        => await _context.PortfolioChanges.AddAsync(change, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
