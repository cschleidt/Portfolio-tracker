using PortfolioTracker.Application.DTOs;

namespace PortfolioTracker.Application.Interfaces;

public interface IPortfolioScraperService
{
    Task<IEnumerable<ScrapedInvestorDto>> ScrapeInvestorsAsync(CancellationToken cancellationToken = default);
    Task<ScrapedPortfolioDto?> ScrapePortfolioAsync(string investorExternalId, CancellationToken cancellationToken = default);
}
