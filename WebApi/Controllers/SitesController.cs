using MediatR;
using Microsoft.AspNetCore.Mvc;
using BMS.Application.Location.Sites.Commands;
using BMS.Application.Location.Sites.Queries;
using Bms.Infrastructure.Seeds;
using WebApi.Security.Authorization;

namespace BMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SitesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SitesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET LIST
    [HttpGet]
    [RequirePermission(PermissionKeys.Sites.View)]
    public async Task<IActionResult> GetList([FromQuery] GetSitesListQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // GET BY ID
    [HttpGet("{id:guid}")]
    [RequirePermission(PermissionKeys.Sites.View)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetSiteByIdQuery(id));

        if (result is null)
            return NotFound();

        return Ok(result);
    }
    // CREATE
    [HttpPost]
    [RequirePermission(PermissionKeys.Sites.Create)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateSiteCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    // UPDATE
    [HttpPut("{id:guid}")]
    [RequirePermission(PermissionKeys.Sites.Update)]
    public async Task<IActionResult> Update(Guid id, UpdateSiteCommand command)
    {
        if (id != command.Id)
            return BadRequest("Id mismatch.");

        await _mediator.Send(command);
        return NoContent();
    }

    // DELETE
    [RequirePermission(PermissionKeys.Sites.Delete)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteSiteCommand
        {
            Id = id
        });

        return NoContent();
    }

}
