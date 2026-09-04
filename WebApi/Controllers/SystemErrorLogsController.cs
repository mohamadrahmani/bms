using System.Security.Claims;
using Bms.Infrastructure.Seeds;
using BMS.Application.SystemErrorLogs.Commands;
using BMS.Application.SystemErrorLogs.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Security.Authorization;

namespace WebApi.Controllers;

[ApiController]
[Route("api/system-error-logs")]
public sealed class SystemErrorLogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SystemErrorLogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [RequirePermission(PermissionKeys.Logs.SystemErrorLogsView)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetSystemErrorLogsQuery query,
        CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    [RequirePermission(PermissionKeys.Logs.SystemErrorLogsView)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetSystemErrorLogByIdQuery(id),
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpPatch("{id:guid}/resolve")]
    [RequirePermission(PermissionKeys.Logs.SystemErrorLogsResolve)]
    public async Task<IActionResult> Resolve(
        Guid id,
        [FromBody] ResolveSystemErrorLogRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var resolvedByUserId = Guid.TryParse(userId, out var parsedUserId)
            ? parsedUserId
            : (Guid?)null;

        var resolved = await _mediator.Send(
            new ResolveSystemErrorLogCommand(
                id,
                resolvedByUserId,
                request.ResolutionNote),
            cancellationToken);

        return resolved ? NoContent() : NotFound();
    }
}

public sealed class ResolveSystemErrorLogRequest
{
    public string? ResolutionNote { get; set; }
}
