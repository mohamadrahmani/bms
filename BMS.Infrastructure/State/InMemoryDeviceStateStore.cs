using BMS.Application.Interfaces;
using BMS.Domain.Entities.BMS;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BMS.Application.Abstraction;

namespace BMS.Infrastructure.State
{
    public class InMemoryDeviceStateStore : IDeviceStateStore
    {
        private readonly ConcurrentDictionary<Guid, Device> _devices = new();

        public Device GetOrCreate(Guid deviceId)
        {
            return _devices.GetOrAdd(deviceId, id => new Device(id));
        }

        public void Update(Guid deviceId, Guid pointId, object? value)
        {
            var device = GetOrCreate(deviceId);
            device.UpdatePoint(pointId, value);
        }

        public Device Get(Guid id)
    => _devices.GetOrAdd(id, _ => new Device(id));

        //public IReadOnlyCollection<Device> GetAll() => _devices.Values;
    }
}
