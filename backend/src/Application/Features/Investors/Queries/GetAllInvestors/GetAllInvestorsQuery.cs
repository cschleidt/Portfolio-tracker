using MediatR;
using PortfolioTracker.Application.DTOs;

namespace PortfolioTracker.Application.Features.Investors.Queries.GetAllInvestors;

public record GetAllInvestorsQuery : IRequest<IEnumerable<InvestorDto>>;
