using BMS.Application.Controllers.Queries;
using BMS.Application.Location.Wards.Commands;
using BMS.Application.Location.Wards.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class WardsController : ControllerBase
{
    private readonly IMediator _mediator;

    public WardsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetControllersListQuery query)
    {
        return Ok(await _mediator.Send(query));
    }





    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        return Ok(await _mediator.Send(new GetWardByIdQuery { Id = id }));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateWardCommand command)
    {
        var id = await _mediator.Send(command);

        return Ok(id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateWardCommand command)
    {
        command.Id = id;

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteWardCommand { Id = id });

        return NoContent();
    }
}
