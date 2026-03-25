using PortfolioTracker.Domain.Entities;

namespace PortfolioTracker.Application.DTOs;

public record PortfolioChangeDto(
    Guid Id,
    Guid PortfolioId,
    string InvestorName,
    ChangeType ChangeType,
    string Ticker,
    string Description,
    decimal? OldValue,
    decimal? NewValue,
    DateTime DetectedAt
);
