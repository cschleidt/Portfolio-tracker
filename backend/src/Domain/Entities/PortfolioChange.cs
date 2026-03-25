using PortfolioTracker.Domain.Common;

namespace PortfolioTracker.Domain.Entities;

public enum ChangeType
{
    HoldingAdded,
    HoldingRemoved,
    QuantityIncreased,
    QuantityDecreased,
    PerformanceUpdated
}

public class PortfolioChange : BaseEntity
{
    public Guid PortfolioId { get; private set; }
    public Portfolio Portfolio { get; private set; } = null!;
    public ChangeType ChangeType { get; private set; }
    public string Ticker { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal? OldValue { get; private set; }
    public decimal? NewValue { get; private set; }
    public DateTime DetectedAt { get; private set; }

    private PortfolioChange() { }

    public static PortfolioChange Create(
        Guid portfolioId, ChangeType changeType, string ticker,
        string description, decimal? oldValue = null, decimal? newValue = null)
        => new()
        {
            PortfolioId = portfolioId,
            ChangeType = changeType,
            Ticker = ticker,
            Description = description,
            OldValue = oldValue,
            NewValue = newValue,
            DetectedAt = DateTime.UtcNow
        };
}
