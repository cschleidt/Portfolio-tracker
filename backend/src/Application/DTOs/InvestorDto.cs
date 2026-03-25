namespace PortfolioTracker.Application.DTOs;

public record InvestorDto(
    Guid Id,
    string Name,
    string Description,
    string? ImageUrl,
    decimal TotalPortfolioValue,
    decimal PerformancePercent,
    DateTime LastSyncedAt,
    int HoldingsCount
);
