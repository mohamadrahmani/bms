using BMS.Application.Interfaces;
using BMS.Application.Models;
using BMS.Domain.Events;
using BMS.Application.Interfaces;  
using BMS.Application.Models;
using BMS.Domain.Events;
using System.Threading.Tasks;

namespace BMS.Infrastructure.Historian
{
    public class DataPointUpdatedHistorianHandler :
        IEventHandler<DataPointUpdatedDomainEvent>
    {
        private readonly IHistorianWriter _writer;

        public DataPointUpdatedHistorianHandler(
            IHistorianWriter writer)
        {
            _writer = writer;
        }

        public Task HandleAsync(
            DataPointUpdatedDomainEvent domainEvent)
        {
            var delta = new DataPointDeltaModel(
                domainEvent.DeviceId,
                domainEvent.PointId,
                domainEvent.Value,
                domainEvent.TimestampUtc);

            _writer.Enqueue(delta);

            return Task.CompletedTask;
        }
    }
}