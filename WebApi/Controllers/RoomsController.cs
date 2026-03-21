using MediatR;
using Microsoft.AspNetCore.Mvc;
using BMS.Application.Location.Rooms.Commands;
using BMS.Application.Location.Rooms.Queries;
using BMS.Application.Location.Rooms.Dtos;

namespace BMS.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RoomsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/rooms
    [HttpGet]
    public async Task<ActionResult<List<RoomDto>>> GetAll([FromQuery] GetAllRoomsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // GET: api/rooms/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<RoomDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetRoomByIdQuery(id));

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    // GET: api/rooms/ward/{wardId}
    [HttpGet("ward/{wardId}")]
    public async Task<ActionResult<List<RoomDto>>> GetByWard(Guid wardId)
    {
        var result = await _mediator.Send(new GetRoomsByWardQuery(wardId));
        return Ok(result);
    }

    // POST: api/rooms
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateRoomCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    // PUT: api/rooms/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateRoomCommand command)
    {
        if (id != command.Id)
            return BadRequest();

        await _mediator.Send(command);
        return NoContent();
    }

    // DELETE: api/rooms/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteRoomCommand(id));
        return NoContent();
    }
}
