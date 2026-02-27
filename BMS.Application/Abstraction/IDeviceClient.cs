using BMS.Application.Models;

namespace BMS.Application.Abstractions;

public interface IDeviceClient
{
    Guid DeviceId { get; }
    Task<DeviceSnapshotDto> ReadAsync(CancellationToken cancellationToken);
    Task<bool> WriteAsync (string pointCode,double EngineeringValue, CancellationToken cancellationToken);
}
