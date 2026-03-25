using PortfolioTracker.Domain.Common;

namespace PortfolioTracker.Domain.Entities;

public class Holding : BaseEntity
{
    public Guid PortfolioId { get; private set; }
    public Portfolio Portfolio { get; private set; } = null!;
    public string Ticker { get; private set; } = string.Empty;
    public string CompanyName { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public decimal EntryPrice { get; private set; }
    public decimal CurrentPrice { get; private set; }
    public decimal MarketValue { get; private set; }
    public decimal WeightPercent { get; private set; }
    public decimal ChangePercent { get; private set; }
    public string Currency { get; private set; } = "DKK";

    private Holding() { }

    public static Holding Create(
        Guid portfolioId, string ticker, string companyName,
        decimal quantity, decimal entryPrice, decimal currentPrice,
        string currency = "DKK")
    {
        var marketValue = quantity * currentPrice;
        var changePercent = entryPrice > 0 ? ((currentPrice - entryPrice) / entryPrice) * 100 : 0;
        return new()
        {
            PortfolioId = portfolioId,
            Ticker = ticker,
            CompanyName = companyName,
            Quantity = quantity,
            EntryPrice = entryPrice,
            CurrentPrice = currentPrice,
            MarketValue = marketValue,
            ChangePercent = changePercent,
            Currency = currency
        };
    }

    public void SetWeight(decimal weightPercent)
    {
        WeightPercent = weightPercent;
        UpdatedAt = DateTime.UtcNow;
    }
}
