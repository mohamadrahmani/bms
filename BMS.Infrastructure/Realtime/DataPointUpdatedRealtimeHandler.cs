using BMS.Application.Interfaces;
using BMS.Domain.Events;
using BMS.Infrastructure.Realtime;
using Microsoft.AspNetCore.SignalR;
using BMS.Application.Interfaces;
using BMS.Domain.Events;
using System.Threading.Tasks;
using BMS.Infrastructure.Realtime.Hubs;

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

        public async Task HandleAsync(
            DataPointUpdatedDomainEvent domainEvent)
        {
            var dto = new RealtimeDataPointDto(
                domainEvent.DeviceId,
                domainEvent.PointId,
                domainEvent.Value,
                domainEvent.TimestampUtc);

            await _hubContext
                .Clients.All
                //.Group(domainEvent.DeviceId)
                .SendAsync("datapointUpdated", dto);
        }
    }
}