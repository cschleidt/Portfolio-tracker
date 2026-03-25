using MediatR;
using PortfolioTracker.Application.DTOs;
using PortfolioTracker.Application.Interfaces;

namespace PortfolioTracker.Application.Features.Investors.Queries.GetAllInvestors;

public class GetAllInvestorsHandler : IRequestHandler<GetAllInvestorsQuery, IEnumerable<InvestorDto>>
{
    private readonly IInvestorRepository _investorRepository;
    private readonly IPortfolioRepository _portfolioRepository;

    public GetAllInvestorsHandler(IInvestorRepository investorRepository, IPortfolioRepository portfolioRepository)
    {
        _investorRepository = investorRepository;
        _portfolioRepository = portfolioRepository;
    }

    public async Task<IEnumerable<InvestorDto>> Handle(GetAllInvestorsQuery request, CancellationToken cancellationToken)
    {
        var investors = await _investorRepository.GetAllAsync(cancellationToken);
        var result = new List<InvestorDto>();

        foreach (var investor in investors)
        {
            var portfolio = await _portfolioRepository.GetByInvestorIdAsync(investor.Id, cancellationToken);
            result.Add(new InvestorDto(
                investor.Id,
                investor.Name,
                investor.Description,
                investor.ImageUrl,
                portfolio?.TotalValue ?? 0,
                portfolio?.PerformancePercent ?? 0,
                portfolio?.LastSyncedAt ?? investor.CreatedAt,
                portfolio?.Holdings.Count ?? 0
            ));
        }

        return result;
    }
}
