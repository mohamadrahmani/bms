using Bms.Infrastructure.Seeds;
using BMS.Application.Location.Floors.Commands;
using BMS.Application.Location.Floors.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Security.Authorization;

[ApiController]
public class FloorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public FloorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET floors by building
    [RequirePermission(PermissionKeys.Floors.View)]
    [HttpGet("api/buildings/{buildingId}/floors")]
    public async Task<IActionResult> GetByBuilding(Guid buildingId)
    {
        var result = await _mediator.Send(
            new GetFloorsByBuildingQuery { BuildingId = buildingId });

        return Ok(result);
    }

    // GET all floors
    [RequirePermission(PermissionKeys.Floors.View)]
    [HttpGet("api/floors")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllFloorsQuery query)
    {
        var result = await _mediator.Send(query);

        return Ok(result);
    }


    // GET floor by id
    [RequirePermission(PermissionKeys.Floors.View)]
    [HttpGet("api/floors/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(
            new GetFloorByIdQuery { Id = id });

        return Ok(result);
    }


    // CREATE floor under building
    [RequirePermission(PermissionKeys.Floors.Create)]
    [HttpPost("api/buildings/{buildingId}/floors")]
    public async Task<IActionResult> Create(
        Guid buildingId,
        CreateFloorCommand command)
    {
        command.BuildingId = buildingId;

        var id = await _mediator.Send(command);

        return Ok(id);
    }

    // UPDATE floor
    [RequirePermission(PermissionKeys.Floors.Update)]
    [HttpPut("api/floors/{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateFloorCommand command)
    {
        command.Id = id;

        await _mediator.Send(command);

        return NoContent();
    }


    // DELETE floor
    [RequirePermission(PermissionKeys.Floors.Delete)]
    [HttpDelete("api/floors/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(
            new DeleteFloorCommand { Id = id });

        return NoContent();
    }
}
