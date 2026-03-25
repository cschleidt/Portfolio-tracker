using Microsoft.EntityFrameworkCore;
using PortfolioTracker.Application.Interfaces;
using PortfolioTracker.Domain.Entities;
using PortfolioTracker.Infrastructure.Persistence;

namespace PortfolioTracker.Infrastructure.Repositories;

public class InvestorRepository : IInvestorRepository
{
    private readonly AppDbContext _context;

    public InvestorRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Investor>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Investors.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Investor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Investors.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<Investor?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default)
        => await _context.Investors.AsNoTracking().FirstOrDefaultAsync(x => x.ExternalId == externalId, cancellationToken);

    public async Task AddAsync(Investor investor, CancellationToken cancellationToken = default)
        => await _context.Investors.AddAsync(investor, cancellationToken);

    public Task UpdateAsync(Investor investor, CancellationToken cancellationToken = default)
    {
        _context.Investors.Update(investor);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
