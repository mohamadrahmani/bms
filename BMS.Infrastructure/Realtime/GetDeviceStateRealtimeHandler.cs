using BMS.Application.Interfaces;
using BMS.Domain.Events;
using BMS.Infrastructure.Realtime;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using BMS.Infrastructure.Realtime.Hubs;
using BMS.Domain.Entities.BMS;

namespace BMS.Infrastructure.Realtime
{
    public class GetDeviceStateRealtimeHandler :
        IEventHandler<GetDeviceStateDomainEvent>
    {
        private readonly IHubContext<BMSHub> _hubContext;

        public GetDeviceStateRealtimeHandler(
            IHubContext<BMSHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task HandleAsync(
            GetDeviceStateDomainEvent domainEvent)
        {
            var dto = new { DeviceId= domainEvent.DeviceId, domainEvent.device.DevicePoints };

            await _hubContext
                .Clients.All
                //.Group(domainEvent.DeviceId)
                .SendAsync("GetDeviceState", dto);
        }
    }
}