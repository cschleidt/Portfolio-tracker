using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PortfolioTracker.Application.Interfaces;
using PortfolioTracker.Domain.Entities;
using PortfolioTracker.Infrastructure.Persistence;

namespace PortfolioTracker.Infrastructure.BackgroundServices;

public class PortfolioSyncBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PortfolioSyncBackgroundService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(30);

    public PortfolioSyncBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<PortfolioSyncBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Portfolio sync started. Interval: {Interval}", _interval);

        await SyncPortfoliosAsync(stoppingToken);

        using var timer = new PeriodicTimer(_interval);
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            await SyncPortfoliosAsync(stoppingToken);
        }
    }

    private async Task SyncPortfoliosAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting portfolio sync at {Time}", DateTime.UtcNow);

        using var scope = _scopeFactory.CreateScope();
        var scraper = scope.ServiceProvider.GetRequiredService<IPortfolioScraperService>();
        var investorRepo = scope.ServiceProvider.GetRequiredService<IInvestorRepository>();
        var portfolioRepo = scope.ServiceProvider.GetRequiredService<IPortfolioRepository>();
        var changeRepo = scope.ServiceProvider.GetRequiredService<IPortfolioChangeRepository>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            var scrapedInvestors = await scraper.ScrapeInvestorsAsync(cancellationToken);

            foreach (var scrapedInvestor in scrapedInvestors)
            {
                var investor = await investorRepo.GetByExternalIdAsync(scrapedInvestor.ExternalId, cancellationToken);

                if (investor is null)
                {
                    investor = Investor.Create(scrapedInvestor.Name, scrapedInvestor.Description, scrapedInvestor.ImageUrl, scrapedInvestor.ExternalId);
                    await investorRepo.AddAsync(investor, cancellationToken);
                    await investorRepo.SaveChangesAsync(cancellationToken);

                    var newPortfolio = Portfolio.Create(investor.Id);
                    await portfolioRepo.AddAsync(newPortfolio, cancellationToken);
                    await portfolioRepo.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    investor.Update(scrapedInvestor.Name, scrapedInvestor.Description, scrapedInvestor.ImageUrl);
                    await investorRepo.UpdateAsync(investor, cancellationToken);
                    await investorRepo.SaveChangesAsync(cancellationToken);
                }

                var scrapedPortfolio = await scraper.ScrapePortfolioAsync(scrapedInvestor.ExternalId, cancellationToken);
                if (scrapedPortfolio is null) continue;

                var existingPortfolio = await dbContext.Portfolios
                    .Include(p => p.Holdings)
                    .FirstOrDefaultAsync(p => p.InvestorId == investor.Id, cancellationToken);

                if (existingPortfolio is null) continue;

                // Detect changes in holdings
                var existingTickers = existingPortfolio.Holdings.ToDictionary(h => h.Ticker);
                var newTickers = scrapedPortfolio.Holdings.ToDictionary(h => h.Ticker);

                foreach (var added in newTickers.Keys.Except(existingTickers.Keys))
                {
                    var holding = newTickers[added];
                    var change = PortfolioChange.Create(
                        existingPortfolio.Id, ChangeType.HoldingAdded, added,
                        $"{investor.Name} har tilføjet {holding.CompanyName} til porteføljen",
                        null, holding.CurrentPrice);
                    await changeRepo.AddAsync(change, cancellationToken);
                    await changeRepo.SaveChangesAsync(cancellationToken);
                    await notificationService.NotifyPortfolioChangedAsync(change, investor.Name, cancellationToken);
                }

                foreach (var removed in existingTickers.Keys.Except(newTickers.Keys))
                {
                    var holding = existingTickers[removed];
                    var change = PortfolioChange.Create(
                        existingPortfolio.Id, ChangeType.HoldingRemoved, removed,
                        $"{investor.Name} har fjernet {holding.CompanyName} fra porteføljen",
                        holding.CurrentPrice, null);
                    await changeRepo.AddAsync(change, cancellationToken);
                    await changeRepo.SaveChangesAsync(cancellationToken);
                    await notificationService.NotifyPortfolioChangedAsync(change, investor.Name, cancellationToken);
                }

                // Replace holdings and recalculate weights
                dbContext.Holdings.RemoveRange(existingPortfolio.Holdings);
                await dbContext.SaveChangesAsync(cancellationToken);

                var totalValue = scrapedPortfolio.Holdings.Sum(h => h.Quantity * h.CurrentPrice);
                var newHoldings = new List<Holding>();

                foreach (var sh in scrapedPortfolio.Holdings)
                {
                    var holding = Holding.Create(
                        existingPortfolio.Id, sh.Ticker, sh.CompanyName,
                        sh.Quantity, sh.EntryPrice, sh.CurrentPrice, sh.Currency);
                    var weight = totalValue > 0 ? (sh.Quantity * sh.CurrentPrice / totalValue) * 100 : 0;
                    holding.SetWeight(Math.Round(weight, 4));
                    newHoldings.Add(holding);
                }

                await dbContext.Holdings.AddRangeAsync(newHoldings, cancellationToken);
                existingPortfolio.UpdateStats(totalValue, scrapedPortfolio.PerformancePercent);
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation("Portfolio sync completed at {Time}", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Portfolio sync failed at {Time}", DateTime.UtcNow);
        }
    }
}
