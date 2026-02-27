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
        string deviceId,
        string commandName,
        object? payload)
    {
        var commandId = await _useCase.ExecuteAsync(
            deviceId,
            commandName,
        payload);

        await eventDispatcher.DispatchAsync(
        new DeviceCommandCompletedDomainEvent(
            "start",
            deviceId,
            commandName,
            true,
            null,
            DateTime.UtcNow)
        );

        return Ok(new { commandId });
    }
}