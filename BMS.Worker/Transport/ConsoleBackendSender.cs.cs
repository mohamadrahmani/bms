using BMS.Worker.Abstractions;
using BMS.Application.Models;

namespace BMS.Worker.Transport;

public class ConsoleBackendSender : IBackendSender
{
    public Task SendAsync(DeviceSnapshotDto snapshot, CancellationToken cancellationToken)
    {
        //foreach (var item in snapshot.Sensors)
        //{
        //    Console.WriteLine($"Device: {snapshot.DeviceId}");
        //    Console.WriteLine($"SensorId: {item.SensorId}");
        //    Console.WriteLine($"Time: {snapshot.Timestamp}");
        //    Console.WriteLine($"SensorsCount: {snapshot.Sensors.Count}");
        //    Console.WriteLine($"Value: {item.Value}");
        //    Console.WriteLine($"SensorsName: {item.Name}");
        //    Console.WriteLine($"SensorAddress: {item.Address}");
        //    Console.WriteLine("---------------------------------");
        //}


        return Task.CompletedTask;
    }
}
