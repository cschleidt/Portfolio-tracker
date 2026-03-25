namespace PortfolioTracker.Application.DTOs;

public record ScrapedInvestorDto(
    string ExternalId,
    string Name,
    string Description,
    string? ImageUrl
);

public record ScrapedHoldingDto(
    string Ticker,
    string CompanyName,
    decimal Quantity,
    decimal EntryPrice,
    decimal CurrentPrice,
    string Currency
);

public record ScrapedPortfolioDto(
    string InvestorExternalId,
    decimal TotalValue,
    decimal PerformancePercent,
    IEnumerable<ScrapedHoldingDto> Holdings
);
