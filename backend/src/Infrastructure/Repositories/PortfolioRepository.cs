using Microsoft.EntityFrameworkCore;
using PortfolioTracker.Application.Interfaces;
using PortfolioTracker.Domain.Entities;
using PortfolioTracker.Infrastructure.Persistence;

namespace PortfolioTracker.Infrastructure.Repositories;

public class PortfolioRepository : IPortfolioRepository
{
    private readonly AppDbContext _context;

    public PortfolioRepository(AppDbContext context) => _context = context;

    public async Task<Portfolio?> GetByInvestorIdAsync(Guid investorId, CancellationToken cancellationToken = default)
        => await _context.Portfolios
            .Include(p => p.Holdings)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.InvestorId == investorId, cancellationToken);

    public async Task<Portfolio?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Portfolios
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task AddAsync(Portfolio portfolio, CancellationToken cancellationToken = default)
        => await _context.Portfolios.AddAsync(portfolio, cancellationToken);

    public Task UpdateAsync(Portfolio portfolio, CancellationToken cancellationToken = default)
    {
        _context.Portfolios.Update(portfolio);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
