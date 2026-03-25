using MediatR;
using PortfolioTracker.Application.DTOs;

namespace PortfolioTracker.Application.Features.Investors.Queries.GetInvestorById;

public record GetInvestorByIdQuery(Guid Id) : IRequest<InvestorDto?>;
