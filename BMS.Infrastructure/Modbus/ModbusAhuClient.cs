using BMS.Application.Abstractions;
using BMS.Application.Models;
using NModbus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Infrastructure.Modbus;

public class ModbusAhuClient : IDeviceClient
{
    private readonly IModbusConnectionManager _connectionManager;
    private readonly byte _slaveId = 1;

    public ModbusAhuClient(IModbusConnectionManager connectionManager)
    {
        _connectionManager = connectionManager;
    }

    public async Task<DeviceSnapshotDto> ReadAsync(CancellationToken cancellationToken)
    {
        var master = await _connectionManager.GetMasterAsync(cancellationToken);

        var registers = await _connectionManager.ExecuteWithRetryAsync(() =>
            master.ReadHoldingRegistersAsync(_slaveId, 0, 2)
        );


        return new DeviceSnapshotDto
        {
            DeviceId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Timestamp = DateTime.UtcNow,
            Sensors =
            {
                new() { SensorId = Guid.NewGuid(), Value = registers[0] },
                new() { SensorId = Guid.NewGuid(), Value = registers[1] }
            }
        };
    }
}

