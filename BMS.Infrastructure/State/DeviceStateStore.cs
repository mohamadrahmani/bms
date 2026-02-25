using BMS.Application.Interfaces;
using BMS.Domain.Entities;
using System.Collections.Concurrent;
namespace BMS.Infrastructure.Historian;
public class DeviceStateStore : IDeviceStateStore
{
    private readonly ConcurrentDictionary<string, Device> _devices = new();

    public Device Get(string id)
        => _devices.GetOrAdd(id, _ => new Device(id));
}