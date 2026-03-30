using BMS.Application.Models;
using BMS.Application.Points.Dtos;

namespace BMS.Application.Abstractions;

public interface IDeviceClient
{
    Guid DeviceId { get; }
    Task<DeviceSnapshotDto> ReadAsync(CancellationToken cancellationToken);
    //Task<bool> WriteAsync (PointDto pointCode,double EngineeringValue, CancellationToken cancellationToken);
}
