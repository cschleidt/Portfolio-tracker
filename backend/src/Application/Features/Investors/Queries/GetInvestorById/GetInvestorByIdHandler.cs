using MediatR;
using PortfolioTracker.Application.DTOs;
using PortfolioTracker.Application.Interfaces;

namespace PortfolioTracker.Application.Features.Investors.Queries.GetInvestorById;

public class GetInvestorByIdHandler : IRequestHandler<GetInvestorByIdQuery, InvestorDto?>
{
    private readonly IInvestorRepository _investorRepository;
    private readonly IPortfolioRepository _portfolioRepository;

    public GetInvestorByIdHandler(IInvestorRepository investorRepository, IPortfolioRepository portfolioRepository)
    {
        _investorRepository = investorRepository;
        _portfolioRepository = portfolioRepository;
    }

    public async Task<InvestorDto?> Handle(GetInvestorByIdQuery request, CancellationToken cancellationToken)
    {
        var investor = await _investorRepository.GetByIdAsync(request.Id, cancellationToken);
        if (investor is null) return null;

        var portfolio = await _portfolioRepository.GetByInvestorIdAsync(investor.Id, cancellationToken);
        return new InvestorDto(
            investor.Id,
            investor.Name,
            investor.Description,
            investor.ImageUrl,
            portfolio?.TotalValue ?? 0,
            portfolio?.PerformancePercent ?? 0,
            portfolio?.LastSyncedAt ?? investor.CreatedAt,
            portfolio?.Holdings.Count ?? 0
        );
    }
}
