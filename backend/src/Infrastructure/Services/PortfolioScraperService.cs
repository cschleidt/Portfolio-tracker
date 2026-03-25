using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using PortfolioTracker.Application.DTOs;
using PortfolioTracker.Application.Interfaces;

namespace PortfolioTracker.Infrastructure.Services;

/// <summary>
/// Scrapes investor portfolios from Saxo Bank's Millionaerklubben campaign page.
/// The page is a JavaScript SPA - if scraping fails (403/no data), falls back to mock data for development.
/// For production, consider using a headless browser (Playwright) or an authenticated API session.
/// </summary>
public class PortfolioScraperService : IPortfolioScraperService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PortfolioScraperService> _logger;
    private const string BaseUrl = "https://www.home.saxo/da-dk/campaigns/millionaerklubben";

    public PortfolioScraperService(HttpClient httpClient, ILogger<PortfolioScraperService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<ScrapedInvestorDto>> ScrapeInvestorsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetStringAsync(BaseUrl, cancellationToken);
            var doc = new HtmlDocument();
            doc.LoadHtml(response);

            // Saxo renders this as a SPA - try common CSS class patterns
            var investorNodes = doc.DocumentNode.SelectNodes(
                "//div[contains(@class,'investor') or contains(@class,'participant') or contains(@class,'member')]");

            if (investorNodes is null || !investorNodes.Any())
            {
                _logger.LogWarning("No investor nodes found on Saxo page - JS rendering required. Using mock data.");
                return GetMockInvestors();
            }

            return investorNodes.Select((node, i) => new ScrapedInvestorDto(
                ExternalId: $"investor-{i}",
                Name: node.SelectSingleNode(".//h3 | .//h2 | .//h4")?.InnerText.Trim() ?? $"Investor {i}",
                Description: node.SelectSingleNode(".//p")?.InnerText.Trim() ?? string.Empty,
                ImageUrl: node.SelectSingleNode(".//img")?.GetAttributeValue("src", null)
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to scrape investors from Saxo. Returning mock data.");
            return GetMockInvestors();
        }
    }

    public async Task<ScrapedPortfolioDto?> ScrapePortfolioAsync(string investorExternalId, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"{BaseUrl}/{investorExternalId}";
            var response = await _httpClient.GetStringAsync(url, cancellationToken);
            var doc = new HtmlDocument();
            doc.LoadHtml(response);

            var holdingNodes = doc.DocumentNode.SelectNodes(
                "//tr[contains(@class,'holding') or contains(@class,'position') or contains(@class,'instrument')]");

            if (holdingNodes is null || !holdingNodes.Any())
            {
                _logger.LogWarning("No holdings found for {InvestorId}. Using mock data.", investorExternalId);
                return GetMockPortfolio(investorExternalId);
            }

            var holdings = holdingNodes
                .Select(node =>
                {
                    var cells = node.SelectNodes(".//td");
                    if (cells is null || cells.Count < 5) return null;
                    return new ScrapedHoldingDto(
                        Ticker: cells[0].InnerText.Trim(),
                        CompanyName: cells[1].InnerText.Trim(),
                        Quantity: decimal.TryParse(cells[2].InnerText.Trim().Replace(".", "").Replace(",", "."), out var qty) ? qty : 0,
                        EntryPrice: decimal.TryParse(cells[3].InnerText.Trim().Replace(".", "").Replace(",", "."), out var entry) ? entry : 0,
                        CurrentPrice: decimal.TryParse(cells[4].InnerText.Trim().Replace(".", "").Replace(",", "."), out var curr) ? curr : 0,
                        Currency: "DKK"
                    );
                })
                .Where(h => h is not null && !string.IsNullOrEmpty(h.Ticker))
                .Cast<ScrapedHoldingDto>();

            return new ScrapedPortfolioDto(investorExternalId, 0, 0, holdings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to scrape portfolio for {InvestorId}. Returning mock data.", investorExternalId);
            return GetMockPortfolio(investorExternalId);
        }
    }

    private static IEnumerable<ScrapedInvestorDto> GetMockInvestors() =>
    [
        new("lars-tvede", "Lars Tvede", "Serieiværksætter, bestsellerforfatter og global makroinvestor", null),
        new("jacob-kirkegaard", "Jacob Kirkegaard", "Økonom og investeringsekspert hos Peterson Institute", null),
        new("anne-buchardt", "Anne Buchardt", "Porteføljeforvalter med fokus på bæredygtige investeringer", null),
        new("peter-nielsen", "Peter Nielsen", "Teknologiinvestor, iværksætter og angel investor", null),
        new("mads-christiansen", "Mads Christiansen", "Professionel trader og teknisk analytiker", null),
    ];

    private static ScrapedPortfolioDto GetMockPortfolio(string externalId)
    {
        var holdingsByInvestor = new Dictionary<string, ScrapedHoldingDto[]>
        {
            ["lars-tvede"] =
            [
                new("NOVO B", "Novo Nordisk B", 500, 650m, 780m, "DKK"),
                new("AAPL", "Apple Inc.", 200, 150m, 175m, "USD"),
                new("MSFT", "Microsoft Corp.", 150, 300m, 420m, "USD"),
                new("NVDA", "NVIDIA Corp.", 100, 450m, 890m, "USD"),
            ],
            ["jacob-kirkegaard"] =
            [
                new("ORSTED", "Ørsted A/S", 300, 500m, 380m, "DKK"),
                new("DSV", "DSV A/S", 100, 1200m, 1450m, "DKK"),
                new("MAERSK B", "A.P. Møller-Mærsk B", 10, 12000m, 13500m, "DKK"),
                new("GS", "Goldman Sachs", 50, 350m, 490m, "USD"),
            ],
            ["anne-buchardt"] =
            [
                new("VESTAS", "Vestas Wind Systems", 400, 180m, 165m, "DKK"),
                new("NESTE", "Neste Oyj", 300, 40m, 35m, "EUR"),
                new("TSLA", "Tesla Inc.", 100, 200m, 175m, "USD"),
                new("AMZN", "Amazon.com Inc.", 80, 140m, 195m, "USD"),
            ],
            ["peter-nielsen"] =
            [
                new("GOOGL", "Alphabet Inc.", 150, 130m, 175m, "USD"),
                new("META", "Meta Platforms Inc.", 200, 300m, 525m, "USD"),
                new("NETCOMPANY", "Netcompany Group", 500, 300m, 380m, "DKK"),
                new("AMBU B", "Ambu B", 1000, 150m, 185m, "DKK"),
            ],
            ["mads-christiansen"] =
            [
                new("CARL B", "Carlsberg B", 200, 900m, 980m, "DKK"),
                new("COLO B", "Coloplast B", 100, 850m, 940m, "DKK"),
                new("JYSK", "Jyske Bank", 300, 450m, 520m, "DKK"),
                new("JPM", "JPMorgan Chase", 100, 155m, 215m, "USD"),
            ],
        };

        var holdings = holdingsByInvestor.TryGetValue(externalId, out var h) ? h :
        [
            new ScrapedHoldingDto("NOVO B", "Novo Nordisk B", 100, 700m, 780m, "DKK"),
            new ScrapedHoldingDto("AAPL", "Apple Inc.", 50, 170m, 175m, "USD"),
        ];

        var totalValue = holdings.Sum(h => h.Quantity * h.CurrentPrice);
        var performance = (decimal)(new Random(externalId.GetHashCode()).NextDouble() * 30 - 5);

        return new ScrapedPortfolioDto(externalId, totalValue, Math.Round(performance, 2), holdings);
    }
}
