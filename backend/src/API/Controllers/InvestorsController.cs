using MediatR;
using Microsoft.AspNetCore.Mvc;
using PortfolioTracker.Application.Features.Investors.Queries.GetAllInvestors;
using PortfolioTracker.Application.Features.Investors.Queries.GetInvestorById;

namespace PortfolioTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvestorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvestorsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var investors = await _mediator.Send(new GetAllInvestorsQuery(), cancellationToken);
        return Ok(investors);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var investor = await _mediator.Send(new GetInvestorByIdQuery(id), cancellationToken);
        return investor is null ? NotFound() : Ok(investor);
    }
}
