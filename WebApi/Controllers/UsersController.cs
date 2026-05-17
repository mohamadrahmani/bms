using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using BMS.Application;
using BMS.Application.Users.Commands;
using BMS.Application.Users.Queries;
using BMS.Application.Users.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Bms.Infrastructure.Seeds;
using WebApi.Security.Authorization;



namespace BMS.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        [HttpPost]
        [RequirePermission(PermissionKeys.Users.Create)]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            var userId = await _mediator.Send(command);
            return Ok(userId);
        }


        [HttpPut("{id:guid}")]
        [RequirePermission(PermissionKeys.Users.Update)]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateUserCommand command)
        {
            command.UserId = id;

            await _mediator.Send(command);

            return Ok(new { success= true });
        }

        [HttpPut("{id:guid}/change-password")]
        [RequirePermission(PermissionKeys.Users.Manage)]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordCommand command)
        {
            command.UserId = id;

            await _mediator.Send(command);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [RequirePermission(PermissionKeys.Users.Delete)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var command = new DeleteUserCommand
            {
                Id = id
            };

            await _mediator.Send(command);

            return NoContent();
        }


        [HttpGet]

        [RequirePermission(PermissionKeys.Users.View)]
        public async Task<IActionResult> GetList([FromQuery] GetUsersListQuery query)
        {
            var users = await _mediator.Send(query);
            return Ok(users);
        }
    }
}

