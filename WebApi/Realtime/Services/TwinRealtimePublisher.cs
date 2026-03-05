using Microsoft.AspNetCore.SignalR;
using WebApi.Realtime.Hubs;
using WebApi.Realtime.Models;

namespace WebApi.Realtime.Services;

public class TwinRealtimePublisher : ITwinRealtimePublisher
{
    private readonly IHubContext<DeviceStateHub> _hubContext;

    public TwinRealtimePublisher(IHubContext<DeviceStateHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task PublishAsync(RealtimeUpdate update)
    {
        await _hubContext
            .Clients
            .All
            .SendAsync("datapointUpdate", update);
    }

    public async Task PublishToDeviceAsync(string deviceId, RealtimeUpdate update)
    {
        await _hubContext
            .Clients
            .Group(deviceId)
            .SendAsync("datapointUpdate", update);
    }
}
