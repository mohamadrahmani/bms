using BMS.Application.Models;
using System.Collections.Concurrent;

public class InMemoryPlcStateStore : IPlcStateStore
{
    // PLC -> (DeviceId -> Snapshot)
    private readonly ConcurrentDictionary<string,
        ConcurrentDictionary<Guid, DeviceSnapshotDto>> _states
        = new();

    public void Update(string plcName, DeviceSnapshotDto snapshot)
    {
        var plcDevices = _states.GetOrAdd(
            plcName,
            _ => new ConcurrentDictionary<Guid, DeviceSnapshotDto>());

        plcDevices[snapshot.DeviceId] = snapshot;
    }

    public IReadOnlyCollection<DeviceSnapshotDto> GetPlcState(string plcName)
    {
        if (_states.TryGetValue(plcName, out var devices))
            return devices.Values.ToList();

        return Array.Empty<DeviceSnapshotDto>();
    }

    public DeviceSnapshotDto? GetDevice(string plcName, Guid deviceId)
    {
        if (_states.TryGetValue(plcName, out var devices))
        {
            devices.TryGetValue(deviceId, out var snapshot);
            return snapshot;
        }

        return null;
    }
}
