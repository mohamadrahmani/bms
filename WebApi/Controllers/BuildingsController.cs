using Bms.Infrastructure.Seeds;
using BMS.Application.Location.Buildings.Commands;
using BMS.Application.Location.Buildings.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Security.Authorization;

[ApiController]
[Route("api/sites/{siteId}/buildings")]
public class BuildingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BuildingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET buildings of a specific site
    [RequirePermission(PermissionKeys.Buildings.View)]
    [HttpGet]
    public async Task<IActionResult> Get(Guid siteId)
    {
        var result = await _mediator.Send(
            new GetBuildingsBySiteQuery { SiteId = siteId });

        return Ok(result);
    }

    // CREATE building under a site
    [RequirePermission(PermissionKeys.Buildings.Create)]
    [HttpPost]
    public async Task<IActionResult> Create(
        Guid siteId,
        CreateBuildingCommand command)
    {
        command.SiteId = siteId;

        var id = await _mediator.Send(command);

        return Ok(id);
    }


    // UPDATE building info
    [HttpPut("{id:guid}")]
    [RequirePermission(PermissionKeys.Buildings.Update)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateBuildingCommand command)
    {
        command.Id = id;

        await _mediator.Send(command);

        return NoContent();
    }

    // DELETE building
    [HttpDelete("{id:guid}")]
    [RequirePermission(PermissionKeys.Buildings.Delete)]
    public async Task<IActionResult> Delete(Guid siteId, Guid id)
    {
        //await _mediator.Send(new DeleteBuildingCommand { Id = id });

        await _mediator.Send(new DeleteBuildingCommand
        {
            Id = id,
            SiteId = siteId
        });

        return NoContent();
    }

    // GET all buildings (independent of site)
    [HttpGet]
    [Route("/api/buildings")]
    [RequirePermission(PermissionKeys.Buildings.View)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllBuildingsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
