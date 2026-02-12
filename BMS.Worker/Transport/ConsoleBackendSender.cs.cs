using BMS.Worker.Abstractions;
using BMS.Application.Models;

namespace BMS.Worker.Transport;

public class ConsoleBackendSender : IBackendSender
{
    public Task SendAsync(DeviceSnapshotDto snapshot, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Device: {snapshot.DeviceId}");
        Console.WriteLine($"Time: {snapshot.Timestamp}");
        Console.WriteLine($"Sensors: {snapshot.Sensors.Count}");
        Console.WriteLine("---------------------------------");

        return Task.CompletedTask;
    }
}
