using BMS.Application.Abstraction;
using BMS.Application.Abstractions;
using BMS.Application.Enum;
using BMS.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public async Task TestReadAsync(CancellationToken token)
    {
        var master = await _connectionManager.GetMasterAsync(token);

        ushort startAddress = 0; // 40001
        ushort numRegisters = 1;

        var registers = await master.ReadHoldingRegistersAsync(1, startAddress, numRegisters);

        Console.WriteLine($"PLC {Name} - 40001 = {registers[0]}");
    }

}

