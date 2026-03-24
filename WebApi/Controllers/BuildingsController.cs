using BMS.Application.Location.Buildings.Commands;
using BMS.Application.Location.Buildings.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/sites/{siteId}/buildings")]
public class BuildingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BuildingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get(Guid siteId)
    {
        var result = await _mediator.Send(
            new GetBuildingsBySiteQuery { SiteId = siteId });

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        Guid siteId,
        CreateBuildingCommand command)
    {
        command.SiteId = siteId;

        var id = await _mediator.Send(command);

        return Ok(id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateBuildingCommand command)
    {
        command.Id = id;

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteBuildingCommand { Id = id });

        return NoContent();
    }
    [HttpGet]
    [Route("/api/buildings")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllBuildingsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
