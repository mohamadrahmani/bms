using Microsoft.AspNetCore.SignalR;

namespace WebApi.Realtime.Hubs;

public class TwinHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    // Optional: Subscribe to specific device/group
    public async Task Subscribe(string deviceId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, deviceId);
    }

    public async Task Unsubscribe(string deviceId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, deviceId);
    }
}
