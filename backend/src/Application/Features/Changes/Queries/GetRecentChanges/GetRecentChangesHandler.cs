using MediatR;
using PortfolioTracker.Application.DTOs;
using PortfolioTracker.Application.Interfaces;

namespace PortfolioTracker.Application.Features.Changes.Queries.GetRecentChanges;

public class GetRecentChangesHandler : IRequestHandler<GetRecentChangesQuery, IEnumerable<PortfolioChangeDto>>
{
    private readonly IPortfolioChangeRepository _changeRepository;
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IInvestorRepository _investorRepository;

    public GetRecentChangesHandler(
        IPortfolioChangeRepository changeRepository,
        IPortfolioRepository portfolioRepository,
        IInvestorRepository investorRepository)
    {
        _changeRepository = changeRepository;
        _portfolioRepository = portfolioRepository;
        _investorRepository = investorRepository;
    }

    public async Task<IEnumerable<PortfolioChangeDto>> Handle(GetRecentChangesQuery request, CancellationToken cancellationToken)
    {
        var changes = await _changeRepository.GetRecentAsync(request.Count, cancellationToken);
        var result = new List<PortfolioChangeDto>();

        foreach (var change in changes)
        {
            var portfolio = await _portfolioRepository.GetByIdAsync(change.PortfolioId, cancellationToken);
            var investor = portfolio is not null
                ? await _investorRepository.GetByIdAsync(portfolio.InvestorId, cancellationToken)
                : null;

            result.Add(new PortfolioChangeDto(
                change.Id,
                change.PortfolioId,
                investor?.Name ?? "Unknown",
                change.ChangeType,
                change.Ticker,
                change.Description,
                change.OldValue,
                change.NewValue,
                change.DetectedAt
            ));
        }

        return result;
    }
}
