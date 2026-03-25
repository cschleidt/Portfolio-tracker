namespace PortfolioTracker.Application.DTOs;

public record HoldingDto(
    Guid Id,
    string Ticker,
    string CompanyName,
    decimal Quantity,
    decimal EntryPrice,
    decimal CurrentPrice,
    decimal MarketValue,
    decimal WeightPercent,
    decimal ChangePercent,
    string Currency
);
