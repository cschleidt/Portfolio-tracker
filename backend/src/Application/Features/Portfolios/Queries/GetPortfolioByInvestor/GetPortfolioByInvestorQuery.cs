using MediatR;
using PortfolioTracker.Application.DTOs;

namespace PortfolioTracker.Application.Features.Portfolios.Queries.GetPortfolioByInvestor;

public record GetPortfolioByInvestorQuery(Guid InvestorId) : IRequest<PortfolioDto?>;
