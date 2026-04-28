using BMS.Domain.Events;
using BMS.Application.Interfaces;
using BMS.Infrastructure.Realtime.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace BMS.Infrastructure.Realtime;

public class CommandCompletedRealtimeHandler :
    IEventHandler<DeviceCommandCompletedDomainEvent>
{
    private readonly IHubContext<BMSHub> _hub;

    public CommandCompletedRealtimeHandler(
        IHubContext<BMSHub> hub)
    {
        _hub = hub;
    }

    public async Task HandleAsync(
        DeviceCommandCompletedDomainEvent domainEvent)
    {
        await _hub.Clients
            //.Group(domainEvent.DeviceId)
            .All
            .SendAsync("commandCompleted", new
            {
                domainEvent.CommandId,
                domainEvent.DeviceId,
                domainEvent.CommandName,
                domainEvent.Success,
                domainEvent.Message,
                domainEvent.CompletedAtUtc
            });
    }
}