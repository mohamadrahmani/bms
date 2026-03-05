using System.Threading.Tasks;
using BMS.Application.Interfaces;
using BMS.Domain.Events;

namespace BMS.Application.UseCases
{
    public class GetDeviceStateUseCase
    {
        private readonly IDeviceStateStore _store;
        private readonly IEventDispatcher _dispatcher;

        public GetDeviceStateUseCase(
            IDeviceStateStore store,
            IEventDispatcher dispatcher)
        {
            _store = store;
            _dispatcher = dispatcher;
        }

        public async Task ExecuteAsync(Guid deviceId)
        {
            var device = _store.Get(deviceId);

            var domainEvent = device.GetDeviceState(deviceId);

            if (domainEvent == null)
                return;

            await _dispatcher.DispatchAsync(domainEvent);
        }
    }
}
