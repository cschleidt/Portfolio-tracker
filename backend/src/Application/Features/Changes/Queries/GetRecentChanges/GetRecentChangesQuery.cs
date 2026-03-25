using MediatR;
using PortfolioTracker.Application.DTOs;

namespace PortfolioTracker.Application.Features.Changes.Queries.GetRecentChanges;

public record GetRecentChangesQuery(int Count = 50) : IRequest<IEnumerable<PortfolioChangeDto>>;
