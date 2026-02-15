using BMS.Application.Abstractions;
using BMS.Application.Models;

namespace BMS.Infrastructure.Modbus;

public class ModbusAhuClient : IDeviceClient
{
    private readonly IModbusConnectionManager _connectionManager;
    private readonly DeviceConfig _deviceConfig;

    public ModbusAhuClient(
        IModbusConnectionManager connectionManager,
        DeviceConfig deviceConfig)
    {
        _connectionManager = connectionManager;
        _deviceConfig = deviceConfig;
    }

    public async Task<DeviceSnapshotDto> ReadAsync(CancellationToken cancellationToken)
    {
        var master = await _connectionManager.GetMasterAsync(cancellationToken);

        var registers = await _connectionManager.ExecuteWithRetryAsync(() =>
            master.ReadHoldingRegistersAsync(
                _deviceConfig.SlaveId,
                _deviceConfig.StartAddress,
                _deviceConfig.RegisterCount)
        );

        var snapshot = new DeviceSnapshotDto
        {
            DeviceId = _deviceConfig.DeviceId,
            Timestamp = DateTime.UtcNow
        };

        foreach (var sensor in _deviceConfig.Sensors)
        {
            snapshot.Sensors.Add(new SensorValueDto
            {
                SensorId = sensor.SensorId,
                Value = registers[sensor.RegisterIndex]
            });
        }

        return snapshot;
    }
}
