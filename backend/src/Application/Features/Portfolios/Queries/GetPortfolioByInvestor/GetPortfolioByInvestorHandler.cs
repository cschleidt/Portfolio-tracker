using MediatR;
using PortfolioTracker.Application.DTOs;
using PortfolioTracker.Application.Interfaces;

namespace PortfolioTracker.Application.Features.Portfolios.Queries.GetPortfolioByInvestor;

public class GetPortfolioByInvestorHandler : IRequestHandler<GetPortfolioByInvestorQuery, PortfolioDto?>
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IInvestorRepository _investorRepository;

    public GetPortfolioByInvestorHandler(IPortfolioRepository portfolioRepository, IInvestorRepository investorRepository)
    {
        _portfolioRepository = portfolioRepository;
        _investorRepository = investorRepository;
    }

    public async Task<PortfolioDto?> Handle(GetPortfolioByInvestorQuery request, CancellationToken cancellationToken)
    {
        var portfolio = await _portfolioRepository.GetByInvestorIdAsync(request.InvestorId, cancellationToken);
        if (portfolio is null) return null;

        var investor = await _investorRepository.GetByIdAsync(request.InvestorId, cancellationToken);

        return new PortfolioDto(
            portfolio.Id,
            portfolio.InvestorId,
            investor?.Name ?? "Unknown",
            portfolio.TotalValue,
            portfolio.PerformancePercent,
            portfolio.LastSyncedAt,
            portfolio.Holdings.Select(h => new HoldingDto(
                h.Id,
                h.Ticker,
                h.CompanyName,
                h.Quantity,
                h.EntryPrice,
                h.CurrentPrice,
                h.MarketValue,
                h.WeightPercent,
                h.ChangePercent,
                h.Currency
            ))
        );
    }
}
