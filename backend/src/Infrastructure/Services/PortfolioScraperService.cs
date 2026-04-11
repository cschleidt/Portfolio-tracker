using System.Globalization;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using PortfolioTracker.Application.DTOs;
using PortfolioTracker.Application.Interfaces;

namespace PortfolioTracker.Infrastructure.Services;

/// <summary>
/// Scrapes investor portfolios from Saxo Bank's Millionaerklubben campaign page.
/// The page is a JavaScript SPA — Playwright is required to fully render it before parsing.
/// Investors: Lars Persson, Lau Svenssen, Michael Friis Jørgensen (in page order).
/// </summary>
public partial class PortfolioScraperService : IPortfolioScraperService
{
    private readonly ILogger<PortfolioScraperService> _logger;
    private const string PageUrl = "https://www.home.saxo/da-dk/campaigns/millionaerklubben";

    public PortfolioScraperService(ILogger<PortfolioScraperService> logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<ScrapedInvestorDto>> ScrapeInvestorsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var html = await FetchRenderedHtmlAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Investor names are h2.highlight that are direct children of div.content
            // (narrows out other h2.highlight headings on the page)
            var nameNodes = doc.DocumentNode.SelectNodes("//div[@class='content']/h2[@class='highlight']");
            var clubNodes = doc.DocumentNode.SelectNodes("//div[@data-qa='millionaires-club']");

            if (nameNodes is null || clubNodes is null || nameNodes.Count == 0)
            {
                _logger.LogWarning("Could not find investor name nodes on Saxo page. Using mock data.");
                return GetMockInvestors();
            }

            var investors = new List<ScrapedInvestorDto>();
            for (int i = 0; i < nameNodes.Count; i++)
            {
                var name = nameNodes[i].InnerText.Trim();
                if (string.IsNullOrEmpty(name)) continue;
                investors.Add(new ScrapedInvestorDto(Slugify(name), name, string.Empty, null));
            }

            if (investors.Count == 0)
            {
                _logger.LogWarning("No investors parsed from Saxo page. Using mock data.");
                return GetMockInvestors();
            }

            _logger.LogInformation("Scraped {Count} investors from Saxo page.", investors.Count);
            return investors;
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
            var html = await FetchRenderedHtmlAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var nameNodes = doc.DocumentNode.SelectNodes("//div[@class='content']/h2[@class='highlight']");
            var clubNodes = doc.DocumentNode.SelectNodes("//div[@data-qa='millionaires-club']");

            if (nameNodes is null || clubNodes is null || nameNodes.Count != clubNodes.Count)
            {
                _logger.LogWarning("Unexpected page structure for investor {Id}. Using mock data.", investorExternalId);
                return GetMockPortfolio(investorExternalId);
            }

            // Match investor by slugified name
            int investorIndex = -1;
            for (int i = 0; i < nameNodes.Count; i++)
            {
                if (Slugify(nameNodes[i].InnerText.Trim()) == investorExternalId)
                {
                    investorIndex = i;
                    break;
                }
            }

            if (investorIndex < 0)
            {
                _logger.LogWarning("Investor {Id} not found on Saxo page. Using mock data.", investorExternalId);
                return GetMockPortfolio(investorExternalId);
            }

            var clubNode = clubNodes[investorIndex];
            var holdings = ParseHoldings(clubNode, investorExternalId);
            var performance = ParseYearPerformance(clubNode);
            var totalValue = holdings.Sum(h => h.Quantity * h.CurrentPrice);

            _logger.LogInformation("Scraped {Count} holdings for {Id}.", holdings.Count, investorExternalId);
            return new ScrapedPortfolioDto(investorExternalId, totalValue, performance, holdings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to scrape portfolio for {Id}. Returning mock data.", investorExternalId);
            return GetMockPortfolio(investorExternalId);
        }
    }

    private static async Task<string> FetchRenderedHtmlAsync()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var page = await browser.NewPageAsync();
        await page.GotoAsync(PageUrl, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
        await page.WaitForSelectorAsync("[data-qa='millionaires-club']",
            new PageWaitForSelectorOptions { Timeout = 15000 });
        return await page.ContentAsync();
    }

    private List<ScrapedHoldingDto> ParseHoldings(HtmlNode clubNode, string investorId)
    {
        // The page renders two table variants; v2-show-sm contains a standard <table>
        var rows = clubNode.SelectNodes(".//div[contains(@class,'v2-show-sm')]//tbody/tr");

        if (rows is null || rows.Count == 0)
        {
            _logger.LogWarning("No holdings rows found for {Id}.", investorId);
            return [];
        }

        var holdings = new List<ScrapedHoldingDto>();
        foreach (var row in rows)
        {
            var cells = row.SelectNodes(".//td");
            if (cells is null || cells.Count < 3) continue;

            var companyName = row.SelectSingleNode(".//div[@class='instrument__description-name']")?.InnerText.Trim() ?? string.Empty;
            var tickerRaw = row.SelectSingleNode(".//span[@class='name']")?.InnerText.Trim() ?? string.Empty;
            // Format is "SYMBOL:exchange" — keep only the symbol part
            var ticker = tickerRaw.Contains(':') ? tickerRaw.Split(':')[0] : tickerRaw;

            // Currency is the 2nd <span> inside instrument__description-exchange
            // Structure: span.name | sup | span(currency) | span.v2-flag
            var exchangeDiv = row.SelectSingleNode(".//div[@class='instrument__description-exchange']");
            var currency = exchangeDiv?.SelectSingleNode("./span[2]")?.InnerText.Trim() ?? "DKK";

            // Danish number format: period = thousands separator, comma = decimal
            var qtyText = cells[1].InnerText.Trim().Replace(".", "").Replace(",", ".");
            var priceText = cells[2].InnerText.Trim().Replace(".", "").Replace(",", ".");

            if (!decimal.TryParse(qtyText, NumberStyles.Any, CultureInfo.InvariantCulture, out var qty)) continue;
            if (!decimal.TryParse(priceText, NumberStyles.Any, CultureInfo.InvariantCulture, out var price)) continue;
            if (string.IsNullOrEmpty(ticker)) continue;

            // Opening price is used for both entry and current (page shows static opening prices)
            holdings.Add(new ScrapedHoldingDto(ticker, companyName, qty, price, price, currency));
        }

        return holdings;
    }

    private static decimal ParseYearPerformance(HtmlNode clubNode)
    {
        var cols = clubNode.SelectNodes(".//div[contains(@class,'portfolio-col--padding')]");
        if (cols is null) return 0;

        foreach (var col in cols)
        {
            var label = col.SelectSingleNode(".//p")?.InnerText.Trim() ?? string.Empty;
            // "Afkast i år" — year-to-date performance
            if (!label.Contains("år", StringComparison.OrdinalIgnoreCase)) continue;

            var valueText = col.SelectSingleNode(".//h4")?.InnerText.Trim()
                .Replace("%", "").Replace(",", ".").Trim();
            if (decimal.TryParse(valueText, NumberStyles.Any, CultureInfo.InvariantCulture, out var perf))
                return perf;
        }

        return 0;
    }

    [GeneratedRegex(@"[^a-z0-9]+")]
    private static partial Regex SlugifyRegex();

    // Slugifies a display name to a stable URL-safe external ID
    // e.g. "Michael Friis Jørgensen" → "michael-friis-jorgensen"
    private static string Slugify(string name) =>
        SlugifyRegex().Replace(
            name.ToLowerInvariant()
                .Replace("ø", "o").Replace("æ", "ae").Replace("å", "a")
                .Replace("ö", "o").Replace("ä", "a").Replace("ü", "u"),
            "-")
            .Trim('-');

    private static IEnumerable<ScrapedInvestorDto> GetMockInvestors() =>
    [
        new("lars-persson", "Lars Persson", string.Empty, null),
        new("lau-svenssen", "Lau Svenssen", string.Empty, null),
        new("michael-friis-jorgensen", "Michael Friis Jørgensen", string.Empty, null),
    ];

    private static ScrapedPortfolioDto GetMockPortfolio(string externalId)
    {
        var holdingsByInvestor = new Dictionary<string, ScrapedHoldingDto[]>
        {
            ["lars-persson"] =
            [
                new("TYRES", "Nokian Renkaat Oyj", 348, 10.22m, 10.22m, "EUR"),
                new("BAVA", "Bavarian Nordic A/S", 75, 148.9m, 148.9m, "DKK"),
                new("VWS", "Vestas Wind Systems A/S", 141, 112.1m, 112.1m, "DKK"),
                new("TRMD_A", "TORM PLC A", 170, 128.9m, 128.9m, "DKK"),
                new("AMBUb", "Ambu A/S", 286, 83.8m, 83.8m, "DKK"),
                new("OKEA", "Okea ASA", 1355, 22.52m, 22.52m, "NOK"),
                new("VISC", "Gruvaktiebolaget Viscaria", 1000, 18.73m, 18.73m, "SEK"),
                new("SWED_A", "Swedbank AB ser A", 120, 341.03m, 341.03m, "SEK"),
                new("VOLVb", "Volvo AB Ser. B", 90, 333.28m, 333.28m, "SEK"),
                new("TEF", "Telefonica SA", 386, 3.7m, 3.7m, "EUR"),
                new("INTRUM", "Intrum AB", 438, 40.46m, 40.46m, "SEK"),
                new("VAR", "Var Energi ASA", 378, 39.44m, 39.44m, "NOK"),
            ],
            ["lau-svenssen"] =
            [
                new("DNORD", "D/S Norden", 235, 318.54m, 318.54m, "DKK"),
                new("PHO", "Photocure ASA", 1000, 58.83m, 58.83m, "NOK"),
                new("TEN_NEW", "Tsakos Energy Navigation", 150, 15.44m, 15.44m, "USD"),
                new("TRMD_A", "TORM PLC A", 170, 132m, 132m, "DKK"),
                new("FRO", "Frontline Plc", 250, 228.7m, 228.7m, "NOK"),
                new("BWLPG", "BW LPG Ltd.", 300, 127.3m, 127.3m, "NOK"),
                new("ZEAL", "Zealand Pharma A/S", 90, 409.5m, 409.5m, "DKK"),
                new("GN", "GN Store Nord A/S", 350, 98.34m, 98.34m, "DKK"),
            ],
            ["michael-friis-jorgensen"] =
            [
                new("BABA", "Alibaba ADR", 12, 87.4m, 87.4m, "USD"),
                new("NKT", "NKT A/S", 20, 452.2m, 452.2m, "DKK"),
                new("SOLARb", "Solar B A/S", 25, 265.5m, 265.5m, "DKK"),
                new("XLFS", "Invesco Financials ETF", 3, 353.28m, 353.28m, "EUR"),
                new("DSV", "DSV A/S", 12, 1336m, 1336m, "DKK"),
                new("NTG", "NTG Nordic Transport Group", 112, 172.4m, 172.4m, "DKK"),
                new("BAVA", "Bavarian Nordic A/S", 50, 173.35m, 173.35m, "DKK"),
                new("PATH", "UiPath Inc.", 218, 14.64m, 14.64m, "USD"),
                new("LOCK", "iShares Digital Security ETF", 156, 8.57m, 8.57m, "EUR"),
                new("NVO", "Novo Nordisk ADR", 100, 46.47m, 46.47m, "USD"),
                new("COLOb", "Coloplast B A/S", 35, 543.4m, 543.4m, "DKK"),
                new("GN", "GN Store Nord A/S", 240, 103.73m, 103.73m, "DKK"),
                new("NNIT", "NNIT A/S", 220, 48.35m, 48.35m, "DKK"),
                new("NOW", "ServiceNow Inc.", 30, 118.22m, 118.22m, "USD"),
            ],
        };

        var holdings = holdingsByInvestor.TryGetValue(externalId, out var h) ? h :
        [
            new ScrapedHoldingDto("VWS", "Vestas Wind Systems A/S", 100, 112.1m, 112.1m, "DKK"),
        ];

        var totalValue = holdings.Sum(h => h.Quantity * h.CurrentPrice);
        return new ScrapedPortfolioDto(externalId, totalValue, 0, holdings);
    }
}
