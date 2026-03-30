using BMS.Application.Abstraction;
using BMS.Application.Abstractions;
using BMS.Application.Enum;
using BMS.Application.Models;
using BMS.Application.Points.Dtos;

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
            try
            {
                var snapshot = await device.ReadAsync(token);
                results.Add(snapshot);


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        return results;
    }

    //public async Task<bool> WriteAsync0(PointDto command, CancellationToken token)
    //{
    //    if (command.ControllerName != Name)
    //        return false;

    //    var device = _devices.FirstOrDefault(d => d.DeviceId == command.DeviceId);
    //    if (device is null)
    //        throw new InvalidOperationException($"Device {command.DeviceId} not found in PLC {Name}");

    //    return await device.WriteAsync(command, double.Parse(command.Value), token);
    //}
    public async Task<bool> WriteAsync(
PointDto point,
double engineeringValue,
CancellationToken token)
    {
        //var point = _config.DevicePoints
        //    .First(p => p.Code == pointCode);

        if (point == null)
            throw new InvalidOperationException($"Point {point.Code} not found.");

        // برای دمو: اگر CommandAddress ست شده باشد، حتی اگر IsWritable فراموش شده باشد اجازه می‌دهیم.
        if (!point.IsWritable)
            throw new InvalidOperationException($"Point {point.Code} is not writable.");

        var master = await _connectionManager.GetMasterAsync(token);

        var registers = ModbusValueParser.BuildWriteRegisters(
            engineeringValue,
            point);

        //var commandAddress = point.CommandAddress ?? point.Address;
        //var feedbackAddress = point.FeedbackAddress ?? commandAddress;
        var readLength = (ushort)Math.Max(point.Length, registers.Length);

        // 1️⃣ Write
        if (registers.Length == 1)
        {
            await _connectionManager.ExecuteWithRetryAsync(() =>
                master.WriteSingleRegisterAsync(
                    1,
                    point.Address.Value,
                    registers[0])
            );
        }
        else
        {
            await _connectionManager.ExecuteWithRetryAsync(() =>
                master.WriteMultipleRegistersAsync(
                    1,
                    point.Address.Value,
                    registers)
            );
        }


        // 2️⃣ Validate
        //for (int i = 0; i < point.ValidationRetryCount; i++)
        //{
        //    await Task.Delay(point.ValidationDelayMs, token);

        //    var feedbackRegisters =
        //        await _connectionManager.ExecuteWithRetryAsync(() =>
        //            master.ReadHoldingRegistersAsync(
        //                1,
        //                feedbackAddress.Value,
        //                readLength));
        //    PointConfig p = new PointConfig
        //    {
        //        DataType = point.DataType,
        //        Scale = point.Scale,
        //        Offset = point.Offset,
        //    };
        //    var feedbackValue =
        //        ModbusValueParser.Parse(feedbackRegisters, p);

        //    if (Math.Abs(feedbackValue - engineeringValue) < 0.01)
        //        return true;
        //}

        return true;
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
