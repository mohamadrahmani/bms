using BMS.Application.Enum;
using BMS.Application.Models;
using BMS.Application.Points.Dtos;


namespace BMS.Application.Abstraction;
public interface IPlcClient
{
    string Name { get; }

    ConnectionState State { get; }

    Task<IEnumerable<DeviceSnapshotDto>> PollAsync(CancellationToken cancellationToken);
    Task<bool> TestConnectionAsync(CancellationToken token);
    Task<bool> WriteAsync(PointDto command,double value, CancellationToken token);

}

