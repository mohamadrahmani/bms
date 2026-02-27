using BMS.Application.Interfaces;
using BMS.Domain.Entities;
using BMS.Domain.Entities.BMS;
using System.Collections.Concurrent;
namespace BMS.Infrastructure.Historian;
public class DeviceStateStore : IDeviceStateStore
{
    private readonly ConcurrentDictionary<Guid, Device> _devices = new();

    public Device Get(Guid id)
        => _devices.GetOrAdd(id, _ => new Device(id));
}