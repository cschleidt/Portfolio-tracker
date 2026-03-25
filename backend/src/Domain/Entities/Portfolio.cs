using PortfolioTracker.Domain.Common;

namespace PortfolioTracker.Domain.Entities;

public class Portfolio : BaseEntity
{
    public Guid InvestorId { get; private set; }
    public Investor Investor { get; private set; } = null!;
    public decimal TotalValue { get; private set; }
    public decimal PerformancePercent { get; private set; }
    public DateTime LastSyncedAt { get; private set; }

    private readonly List<Holding> _holdings = new();
    public IReadOnlyCollection<Holding> Holdings => _holdings.AsReadOnly();

    private readonly List<PortfolioChange> _changes = new();
    public IReadOnlyCollection<PortfolioChange> Changes => _changes.AsReadOnly();

    private Portfolio() { }

    public static Portfolio Create(Guid investorId)
        => new()
        {
            InvestorId = investorId,
            LastSyncedAt = DateTime.UtcNow
        };

    public void UpdateStats(decimal totalValue, decimal performancePercent)
    {
        TotalValue = totalValue;
        PerformancePercent = performancePercent;
        LastSyncedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
