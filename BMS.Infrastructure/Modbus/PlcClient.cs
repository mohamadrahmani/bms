using BMS.Application.Models;
using BMS.Infrastructure.Modbus;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Infrastructure.Modbus;
public class PlcClient
{
    private readonly PlcConfig _config;
    private readonly ModbusConnectionManager _connectionManager;

    public string Name => _config.Name;

    public PlcClient(
        PlcConfig config,
        ILogger<ModbusConnectionManager> logger)
    {
        _config = config;
        _connectionManager =
            new ModbusConnectionManager(
                config.IpAddress,
                config.Port,
                logger);
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
}

