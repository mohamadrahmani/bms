using BMS.Application.Abstractions;
using BMS.Application.Models;
// using BMS.Application.Utilities;

namespace BMS.Infrastructure.Modbus;

public class ModbusAhuClient : IDeviceClient
{
    private readonly IModbusConnectionManager _connectionManager;
    private readonly DeviceConfig _config;
    private readonly string _plcName;
    private readonly Guid _deviceId;

    public ModbusAhuClient(
        string plcName,
        IModbusConnectionManager connectionManager,
        DeviceConfig config)
    {
        _plcName = plcName;
        _connectionManager = connectionManager;
        _config = config;

        // Stable, deterministic identity per PLC+Device
       // _deviceId = DeterministicGuid.FromString($"bms|plc:{_plcName}|device:{_config.Name}");
    }

    public Guid DeviceId => _deviceId;

    public async Task<DeviceSnapshotDto> ReadAsync(
        CancellationToken cancellationToken)
    {
        var master = await _connectionManager.GetMasterAsync(cancellationToken);

        var snapshot = new DeviceSnapshotDto
        {
            DeviceId = DeviceId,
            DeviceName = _config.Name,
            Timestamp = DateTime.UtcNow
        };

        foreach (var point in _config.DevicePoints)
        {
            var registers = await _connectionManager.ExecuteWithRetryAsync(() =>
                master.ReadHoldingRegistersAsync(
                    1,
                    point.Address.Value,
                    point.Length));

            var value = ModbusValueParser.Parse(registers, point);

            snapshot.Sensors.Add(new SensorValueDto
            {
                //SensorId = DeterministicGuid.FromString($"bms|plc:{_plcName}|device:{_config.Name}|point:{point.Code}"),
                Name = point.Code,
                Value = value
            });
        }

        return snapshot;
    }

    public async Task<bool> WriteAsync(
    string pointCode,
    double engineeringValue,
    CancellationToken token)
    {
        var point = _config.DevicePoints
            .First(p => p.Code == pointCode);

        if (point == null)
            throw new InvalidOperationException($"Point {pointCode} not found.");

        // برای دمو: اگر CommandAddress ست شده باشد، حتی اگر IsWritable فراموش شده باشد اجازه می‌دهیم.
        if (!point.IsWritable && point.CommandAddress is null)
            throw new InvalidOperationException($"Point {pointCode} is not writable.");

        var master = await _connectionManager.GetMasterAsync(token);

        var registers = ModbusValueParser.BuildWriteRegisters(
            engineeringValue,
            point);

        var commandAddress = point.CommandAddress ?? point.Address;
        var feedbackAddress = point.FeedbackAddress ?? commandAddress;
        var readLength = (ushort)Math.Max(point.Length, registers.Length);

        // 1️⃣ Write
        if (registers.Length == 1)
        {
            await _connectionManager.ExecuteWithRetryAsync(() =>
                master.WriteSingleRegisterAsync(
                    1,
                    commandAddress.Value,
                    registers[0])
            );
        }
        else
        {
            await _connectionManager.ExecuteWithRetryAsync(() =>
                master.WriteMultipleRegistersAsync(
                    1,
                    commandAddress.Value,
                    registers)
            );
        }


        // 2️⃣ Validate
        for (int i = 0; i < point.ValidationRetryCount; i++)
        {
            await Task.Delay(point.ValidationDelayMs, token);

            var feedbackRegisters =
                await _connectionManager.ExecuteWithRetryAsync(() =>
                    master.ReadHoldingRegistersAsync(
                        1,
                        feedbackAddress.Value,
                        readLength));

            var feedbackValue =
                ModbusValueParser.Parse(feedbackRegisters, point);

            if (Math.Abs(feedbackValue - engineeringValue) < 0.01)
                return true;
        }

        return false;
    }



}

