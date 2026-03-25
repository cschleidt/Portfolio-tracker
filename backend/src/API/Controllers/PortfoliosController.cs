using MediatR;
using Microsoft.AspNetCore.Mvc;
using PortfolioTracker.Application.Features.Portfolios.Queries.GetPortfolioByInvestor;

namespace PortfolioTracker.API.Controllers;

[ApiController]
[Route("api/investors/{investorId:guid}/portfolio")]
public class PortfoliosController : ControllerBase
{
    private readonly IMediator _mediator;

    public PortfoliosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetByInvestor(Guid investorId, CancellationToken cancellationToken)
    {
        var portfolio = await _mediator.Send(new GetPortfolioByInvestorQuery(investorId), cancellationToken);
        return portfolio is null ? NotFound() : Ok(portfolio);
    }
}
