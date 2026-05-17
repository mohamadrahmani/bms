using Azure.Core;
using BMS.Application.Auth.Commands.Login;
using BMS.Application.Auth.Commands.Logout;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi.Models.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        var command = new LoginCommand(
            request.UserName,
            request.Password,
            ip
        );

        var result = await _mediator.Send(command);

        return Ok(result);
    }


    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        Guid.TryParse(userIdStr, out var userId);

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        await _mediator.Send(new LogoutCommand(userId, ip));

        return Ok("خروج با موفقیت انجام شد.");
    }


}
