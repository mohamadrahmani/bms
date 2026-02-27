using BMS.Application.Enum;
using BMS.Application.Models;


namespace BMS.Application.Abstraction;
public interface IPlcClient
{
    string Name { get; }

    ConnectionState State { get; }

    Task<IEnumerable<DeviceSnapshotDto>> PollAsync(CancellationToken cancellationToken);
    Task<bool> TestConnectionAsync(CancellationToken token);
    Task<bool> WriteAsync(WritePointCommand command, CancellationToken token);

}

