using MediatR;
using Microsoft.AspNetCore.Mvc;
using PortfolioTracker.Application.Features.Changes.Queries.GetRecentChanges;

namespace PortfolioTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChangesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChangesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetRecent([FromQuery] int count = 50, CancellationToken cancellationToken = default)
    {
        var changes = await _mediator.Send(new GetRecentChangesQuery(count), cancellationToken);
        return Ok(changes);
    }
}
