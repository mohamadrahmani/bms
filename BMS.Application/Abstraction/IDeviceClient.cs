using BMS.Application.Models;

namespace BMS.Application.Abstractions;

public interface IDeviceClient
{
    Task<DeviceSnapshotDto> ReadAsync(CancellationToken cancellationToken);
}
