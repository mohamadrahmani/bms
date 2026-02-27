using System.Threading.Tasks;
using BMS.Application.Interfaces;
using BMS.Domain.Events;

namespace BMS.Application.UseCases
{
    public class UpdateDataPointUseCase
    {
        private readonly IDeviceStateStore _store;
        private readonly IEventDispatcher _dispatcher;

        public UpdateDataPointUseCase(
            IDeviceStateStore store,
            IEventDispatcher dispatcher)
        {
            _store = store;
            _dispatcher = dispatcher;
        }

        public async Task ExecuteAsync(
            Guid deviceId,
            Guid pointId,
            object? value)
        {
            var device = _store.Get(deviceId);

            var domainEvent = device.UpdatePoint(pointId, value);

            if (domainEvent == null)
                return;

            await _dispatcher.DispatchAsync(domainEvent);
        }
    }
}
