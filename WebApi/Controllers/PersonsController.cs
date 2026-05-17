using Microsoft.AspNetCore.Mvc;
using MediatR;
using BMS.Application.Persons.Commands;
using BMS.Application.Users.Queries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Bms.Infrastructure.Seeds;
using WebApi.Security.Authorization;

namespace BMS.API.Controllers;

//[ApiController]
[Route("api/persons")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PersonsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PersonsController(IMediator mediator)
    {
        _mediator = mediator;
    }


    // CREATE PERSON
    [HttpPost]
    [RequirePermission(PermissionKeys.Persons.Create)]
    public async Task<IActionResult> Create(
        [FromBody] CreatePersonCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    // UPDATE PERSON
    [RequirePermission(PermissionKeys.Persons.Update)]
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


    // GET PERSONS LIST
    [RequirePermission(PermissionKeys.Persons.View)]
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] GetPersonsListQuery query)
    {
        var users = await _mediator.Send(query);
        return Ok(users);
    }


    // DELETE PERSON
    [RequirePermission(PermissionKeys.Persons.Delete)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeletePersonCommand(id));
        return NoContent();
    }

}
