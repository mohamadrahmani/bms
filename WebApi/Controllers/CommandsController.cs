using Microsoft.AspNetCore.Mvc;
using BMS.Application.UseCases;
using BMS.Application.Interfaces;
using BMS.Infrastructure.Events;
using BMS.Domain.Events;

namespace WebApi.Controllers;   

[ApiController]
[Route("api/commands")]
public class CommandsController : ControllerBase    
{
    private readonly ExecuteCommandUseCase _useCase;
    private readonly IEventDispatcher eventDispatcher;
    public CommandsController(IEventDispatcher _eventDispatcher,
        ExecuteCommandUseCase useCase)
    {
        eventDispatcher = _eventDispatcher;
        _useCase = useCase;
    }

    [HttpPost]
    public async Task<IActionResult> Execute(
        [FromBody]  aaaa command)
    {   
        var commandId = await _useCase.ExecuteAsync(
            command.DeviceId,
            command.CommandName,
        command.Value);

        await eventDispatcher.DispatchAsync(
        new DeviceCommandCompletedDomainEvent(
            Guid.Empty,
            command.DeviceId,
            command.CommandName,
            true,
            null,
            DateTime.UtcNow)
        );

        return Ok(new { commandId });
    }
}

public class aaaa
{
    public Guid? DeviceId { get; set; }
    public string CommandName { get; set; }
    public string? Value { get; set; }
}