using BMS.Application.Models;

public interface IPlcStateStore
{
    void Update(string plcName, DeviceSnapshotDto snapshot);
    IReadOnlyCollection<DeviceSnapshotDto> GetPlcState(string plcName);
    DeviceSnapshotDto? GetDevice(string plcName, Guid deviceId);
}