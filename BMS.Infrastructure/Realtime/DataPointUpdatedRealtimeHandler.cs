using BMS.Application.Interfaces;
using BMS.Domain.Events;
using BMS.Infrastructure.Realtime;
using BMS.Application.Interfaces;
using BMS.Domain.Events;
using System.Threading.Tasks;
using BMS.Infrastructure.Realtime.Hubs;
using BMS.Domain.Entities.BMS;
using Microsoft.AspNetCore.SignalR;

namespace BMS.Infrastructure.Realtime
{
    public class DataPointUpdatedRealtimeHandler :
        IEventHandler<DataPointUpdatedDomainEvent>
    {
        private readonly IHubContext<BMSHub> _hubContext;

        public DataPointUpdatedRealtimeHandler(
            IHubContext<BMSHub> hubContext)
        {
            _hubContext = hubContext;
        }

        // 1. متدی برای دریافت پیام از کلاینت
        // نام این متد باید دقیقاً با نامی که در انگولار فراخوانی می‌کنید یکسان باشد
        public async Task GetDeviceState(string deviceId)
        {
            // منطق پردازش پیام دریافتی
            // مثال: لاگ کردن، ذخیره در دیتابیس، یا ارسال به هاب‌های دیگر

            // اگر می‌خواهید پس از دریافت، پیام را به بقیه هم بفرستید:
            // await Clients.All.SendAsync("receiveConfirmation", "Data received");

            System.Console.WriteLine($"Data received for device {deviceId}");
        }

        public async Task HandleAsync(
            DataPointUpdatedDomainEvent domainEvent)
        {
            var dto = new RealtimeDataPointDto(
                domainEvent.DeviceId,
                domainEvent.Id,
                domainEvent.Tag,
                domainEvent.PointType,
                ///domainEvent.CommandType,
                domainEvent.Value,
                domainEvent.TimestampUtc);

            await _hubContext
                .Clients.All
                //.Group(domainEvent.DeviceId)
                .SendAsync("datapointUpdated", dto);
        }
    }
}