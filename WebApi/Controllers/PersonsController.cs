using Microsoft.AspNetCore.Mvc;
using MediatR;
using BMS.Application.Persons.Commands;

namespace BMS.API.Controllers;

[ApiController]
[Route("api/persons")]
public class PersonsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PersonsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePersonCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
    Guid id,
    [FromBody] UpdatePersonCommand command)
    {
        if (id != command.Id)
            return BadRequest();

        await _mediator.Send(command);

        return NoContent();
    }

}
