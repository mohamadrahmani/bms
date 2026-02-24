using BMS.Application.Abstractions;
using BMS.Application.Models;

namespace BMS.Infrastructure.Modbus;

public class ModbusAhuClient : IDeviceClient
{
    private readonly IModbusConnectionManager _connectionManager;
    private readonly DeviceConfig _config;

    public ModbusAhuClient(
        IModbusConnectionManager connectionManager,
        DeviceConfig config)
    {
        _connectionManager = connectionManager;
        _config = config;
    }

    public Guid DeviceId => throw new NotImplementedException();

    public async Task<DeviceSnapshotDto> ReadAsync(
        CancellationToken cancellationToken)
    {
        var master = await _connectionManager.GetMasterAsync(cancellationToken);

        var snapshot = new DeviceSnapshotDto
        {
            DeviceId = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow
        };

        foreach (var point in _config.Points)
        {
            var registers = await _connectionManager.ExecuteWithRetryAsync(() =>
                master.ReadHoldingRegistersAsync(
                    _config.SlaveId,
                    point.Address,
                    point.Length));

            var value = ModbusValueParser.Parse(registers, point);

            snapshot.Sensors.Add(new SensorValueDto
            {
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
        var point = _config.Points
            .FirstOrDefault(p => p.Code == pointCode);

        if (point == null)
            throw new InvalidOperationException($"Point {pointCode} not found.");

        if (!point.IsWritable)
            throw new InvalidOperationException($"Point {pointCode} is not writable.");

        var master = await _connectionManager.GetMasterAsync(token);

        var registers = ModbusValueParser.BuildWriteRegisters(
            engineeringValue,
            point);

        // 1️⃣ Write
        await _connectionManager.ExecuteWithRetryAsync(() =>
            master.WriteSingleRegisterAsync(
                _config.SlaveId,
                point.CommandAddress!.Value,
                registers[0])
        );


        // 2️⃣ Validate
        for (int i = 0; i < point.ValidationRetryCount; i++)
        {
            await Task.Delay(point.ValidationDelayMs, token);

            var feedbackRegisters =
                await _connectionManager.ExecuteWithRetryAsync(() =>
                    master.ReadHoldingRegistersAsync(
                        _config.SlaveId,
                        point.FeedbackAddress!.Value,
                        point.Length));

            var feedbackValue =
                ModbusValueParser.Parse(feedbackRegisters, point);

            if (Math.Abs(feedbackValue - engineeringValue) < 0.01)
                return true;
        }

        return false;
    }



}

