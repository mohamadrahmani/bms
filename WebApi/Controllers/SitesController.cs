using MediatR;
using Microsoft.AspNetCore.Mvc;
using BMS.Application.Location.Sites.Commands;
using BMS.Application.Location.Sites.Queries;

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

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var result = await _mediator.Send(new GetSitesListQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetSiteByIdQuery(id));

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSiteCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateSiteCommand command)
    {
        if (id != command.Id)
            return BadRequest("Id mismatch.");

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteSiteCommand(id));
        return NoContent();
    }
}
