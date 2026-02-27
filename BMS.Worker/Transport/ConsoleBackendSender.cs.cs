using BMS.Worker.Abstractions;
using BMS.Application.Models;

namespace BMS.Worker.Transport;

public class ConsoleBackendSender : IBackendSender
{
    public Task SendAsync(DeviceSnapshotDto snapshot, CancellationToken cancellationToken)
    {
        foreach (var item in snapshot.Sensors)
        {
            Console.WriteLine($"Device: {snapshot.DeviceId}");
            Console.WriteLine($"Time: {snapshot.Timestamp}");
            Console.WriteLine($"Sensors: {snapshot.Sensors.Count}");
            Console.WriteLine($"Value: {item.Value}");
            Console.WriteLine($"Sensors: {item.Name}");
            Console.WriteLine($"Sensors: {item.SensorId}");
            Console.WriteLine("---------------------------------");
        }


        return Task.CompletedTask;
    }
}
