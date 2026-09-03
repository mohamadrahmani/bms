using Bms.Infrastructure.Seeds;
using BMS.Application.Controllers.Queries;
using BMS.Application.Logs.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Security.Authorization;


[ApiController]
[Route("api/logs")]
public class LogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET ALL LOGS
    [RequirePermission(PermissionKeys.Logs.SystemLogsView)]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllLogsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [RequirePermission(PermissionKeys.Logs.SystemLogsView)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetLogByIdQuery(id));
        if (result == null)
            return NotFound();

        return Ok(result);
    }
}
