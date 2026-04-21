using BMS.Application.Interfaces;
using BMS.Application.UseCases;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace BMS.Infrastructure.Realtime.Hubs
{
    public class BMSHub : Hub
    {
        //private readonly GetDeviceStateUseCase _useCase;
        private readonly IDeviceStateStore _store;
        private readonly IEventDispatcher _dispatcher;

        public BMSHub(IDeviceStateStore store, IEventDispatcher dispatcher)
        {
            _store = store;
            _dispatcher = dispatcher;

        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task GetDeviceState(Guid deviceId)
        {
            // منطق پردازش پیام دریافتی
            // مثال: لاگ کردن، ذخیره در دیتابیس، یا ارسال به هاب‌های دیگر

            // اگر می‌خواهید پس از دریافت، پیام را به بقیه هم بفرستید:
            // await Clients.All.SendAsync("receiveConfirmation", "Data received");

            await (new GetDeviceStateUseCase(_store , _dispatcher)).ExecuteAsync(deviceId);

            System.Console.WriteLine($"Data received for device {deviceId}");
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