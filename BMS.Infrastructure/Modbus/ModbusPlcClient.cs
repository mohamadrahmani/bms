using BMS.Application.Abstraction;
using BMS.Application.Abstractions;
using BMS.Application.Enum;
using BMS.Application.Models;

namespace BMS.Infrastructure.Modbus;

public class ModbusPlcClient : IPlcClient
{
    private readonly IModbusConnectionManager _connectionManager;
    private readonly IEnumerable<IDeviceClient> _devices;

    public string Name { get; }

    public ConnectionState State => _connectionManager.State;

    public ModbusPlcClient(
        string name,
        IModbusConnectionManager connectionManager,
        IEnumerable<IDeviceClient> devices)
    {
        Name = name;
        _connectionManager = connectionManager;
        _devices = devices;
    }

    public async Task<bool> TestConnectionAsync(CancellationToken token)
    {
        try
        {
            await _connectionManager.GetMasterAsync(token);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<DeviceSnapshotDto>> PollAsync(CancellationToken token)
    {
        var results = new List<DeviceSnapshotDto>();

        foreach (var device in _devices)
        {
            var snapshot = await device.ReadAsync(token);
            results.Add(snapshot);
        }

        return results;
    }

    public async Task WriteAsync(WritePointCommand command, CancellationToken token)
    {
        if (command.PlcName != Name)
            return;

        var master = await _connectionManager.GetMasterAsync(token);

        await _connectionManager.ExecuteWithRetryAsync(async () =>
        {
            await master.WriteSingleRegisterAsync(
                command.SlaveId,
                command.Address,
                command.Value
            );

            return true;
        });
    }

    // 👇 این متد فقط برای تست ساده است
    //public async Task TestReadAsync(CancellationToken token)
    //{
    //    foreach (var device in _devices)
    //    {
    //        var snapshot = await device.ReadAsync(token);

    //        Console.WriteLine($"PLC: {Name}");
    //        Console.WriteLine($"Device: {snapshot.DeviceId}");
    //        Console.WriteLine($"Time: {snapshot.Timestamp}");

    //        foreach (var sensor in snapshot.Sensors)
    //        {
    //            Console.WriteLine($"  Sensor: {sensor.SensorId} -> {sensor.Value}");
    //        }

    //        Console.WriteLine("----------------------------------");
    //    }
    //}
}
