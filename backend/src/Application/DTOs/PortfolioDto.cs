namespace PortfolioTracker.Application.DTOs;

public record PortfolioDto(
    Guid Id,
    Guid InvestorId,
    string InvestorName,
    decimal TotalValue,
    decimal PerformancePercent,
    DateTime LastSyncedAt,
    IEnumerable<HoldingDto> Holdings
);
