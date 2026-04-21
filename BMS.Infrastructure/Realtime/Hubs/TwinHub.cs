namespace WebApi.Realtime.Hubs;
using Microsoft.AspNetCore.SignalR;

public class DeviceStateHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    public async Task GetDeviceState(string deviceId)
    {
        // منطق پردازش پیام دریافتی
        // مثال: لاگ کردن، ذخیره در دیتابیس، یا ارسال به هاب‌های دیگر

        // اگر می‌خواهید پس از دریافت، پیام را به بقیه هم بفرستید:
        // await Clients.All.SendAsync("receiveConfirmation", "Data received");

        System.Console.WriteLine($"Data received for device {deviceId}");
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
