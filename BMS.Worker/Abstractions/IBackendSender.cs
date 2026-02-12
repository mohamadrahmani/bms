using BMS.Application.Models;

namespace BMS.Worker.Abstractions;

public interface IBackendSender
{
    Task SendAsync(DeviceSnapshotDto snapshot, CancellationToken cancellationToken);
}
