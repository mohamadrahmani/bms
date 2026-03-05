using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using BMS.Application;
using BMS.Application.Users.Commands;
using BMS.Application.Users.Queries;
using BMS.Application.Users.Dtos;
using Microsoft.AspNetCore.Authorization;



namespace BMS.API.Controllers
{

    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            var userId = await _mediator.Send(command);
            return Ok(userId);
        }


        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateUserCommand command)
        {
            command.UserId = id;

            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPut("{id:guid}/change-password")]
        public async Task<IActionResult> ChangePassword(
    Guid id,
    [FromBody] ChangePasswordCommand command)
        {
            command.UserId = id;

            await _mediator.Send(command);

            return NoContent();
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var users = await _mediator.Send(new GetUsersListQuery());
            return Ok(users);
        }
    }
}

