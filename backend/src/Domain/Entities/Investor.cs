using PortfolioTracker.Domain.Common;

namespace PortfolioTracker.Domain.Entities;

public class Investor : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? ImageUrl { get; private set; }
    public string? ExternalId { get; private set; }

    private readonly List<Portfolio> _portfolios = new();
    public IReadOnlyCollection<Portfolio> Portfolios => _portfolios.AsReadOnly();

    private Investor() { }

    public static Investor Create(string name, string description, string? imageUrl = null, string? externalId = null)
        => new()
        {
            Name = name,
            Description = description,
            ImageUrl = imageUrl,
            ExternalId = externalId,
        };

    public void Update(string name, string description, string? imageUrl = null)
    {
        Name = name;
        Description = description;
        ImageUrl = imageUrl;
        UpdatedAt = DateTime.UtcNow;
    }
}
