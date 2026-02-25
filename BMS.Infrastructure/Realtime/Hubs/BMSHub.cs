using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BMS.Infrastructure.Realtime.Hubs
{
    public class BMSHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
        public async Task SubscribeDevice(string deviceId)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                deviceId);
        }

        public async Task UnsubscribeDevice(string deviceId)
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                deviceId);
        }
    }
}