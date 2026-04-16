using BMS.Application.Abstractions;
using BMS.Application.Models;
using BMS.Application.Points.Dtos;
using BMS.Application.Utilities;
using BMS.Domain.Entities.BMS;

namespace BMS.Infrastructure.Modbus;

public class ModbusClient : IDeviceClient
{
    private readonly IModbusConnectionManager _connectionManager;
    private readonly DeviceConfig _config;
    private readonly string _plcName;
    private readonly Guid _deviceId;

    public ModbusClient(
        string plcName,
        IModbusConnectionManager connectionManager,
        DeviceConfig config)
    {
        _plcName = plcName;
        _connectionManager = connectionManager;
        _config = config;

        // Stable, deterministic identity per PLC+Device
        //_deviceId = DeterministicGuid.FromString($"bms|plc:{_plcName}|device:{_config.Name}");
    }

    public Guid DeviceId => _config.Id;
    public string DeviceName => _config.Name;

    public IReadOnlyCollection<PointConfig> Points => _config.DevicePoints;

    //public async Task<DeviceSnapshotDto> ReadAsync(
    //CancellationToken cancellationToken)
    //{
    //    var master = await _connectionManager.GetMasterAsync(cancellationToken);

    //    var snapshot = new DeviceSnapshotDto
    //    {
    //        DeviceId = _config.Id,
    //        DeviceName = _config.Name,
    //        Timestamp = DateTime.UtcNow
    //    };

    //    foreach (var point in _config.DevicePoints)
    //    {
    //        object rawValue;

    //        switch (point.RegisterType)
    //        {
    //            case RegisterType.Coil:
    //                rawValue = await _connectionManager.ExecuteWithRetryAsync(() =>
    //                    master.ReadCoilsAsync(
    //                        1,
    //                        point.Address.Value,
    //                        point.Length));
    //                break;

    //            case RegisterType.DiscreteInput:
    //                rawValue = await _connectionManager.ExecuteWithRetryAsync(() =>
    //                    master.ReadInputsAsync(
    //                        1,
    //                        point.Address.Value,
    //                        point.Length));
    //                break;

    //            case RegisterType.HoldingRegister:
    //                rawValue = await _connectionManager.ExecuteWithRetryAsync(() =>
    //                    master.ReadHoldingRegistersAsync(
    //                        1,
    //                        point.Address.Value,
    //                        point.Length));
    //                break;

    //            case RegisterType.InputRegister:
    //                rawValue = await _connectionManager.ExecuteWithRetryAsync(() =>
    //                    master.ReadInputRegistersAsync(
    //                        1,
    //                        point.Address.Value,
    //                        point.Length));
    //                break;

    //            default:
    //                throw new NotSupportedException($"Register type {point.RegisterType} not supported");
    //        }

    //        var value = ModbusValueParser.Parse(rawValue, point);

    //        snapshot.Sensors.Add(new SensorValueDto
    //        {
    //            SensorId = point.Id,
    //            Name = point.Code,
    //            Value = value,
    //            Address=point.Address.Value
    //        });
    //    }

    //    return snapshot;
    //}


}
