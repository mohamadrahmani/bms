using BMS.Application.Abstraction;
using BMS.Application.Models;
using BMS.Application.Points.Dtos;
using BMS.Worker.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Worker.Devices;
//public class PlcCommandDispatcher : IPlcCommandDispatcher
//{
//    private readonly IEnumerable<IPlcClient> _plcs;

//    public PlcCommandDispatcher(IEnumerable<IPlcClient> plcs)
//    {
//        _plcs = plcs;
//    }

//    public async Task<bool> SendAsync(
//        PointDto command,
//        string value,
//        CancellationToken token)
//    {
//        var plc = _plcs.FirstOrDefault(p => p.Name == command.ControllerName);

//        if (plc == null)
//            throw new InvalidOperationException("PLC not found.");

//        return await plc.WriteAsync(command, double.Parse(value), token);
//    }

//}
public class PlcCommandDispatcher : IPlcCommandDispatcher
{
    private readonly IPlcClientProvider _provider;

    public PlcCommandDispatcher(IPlcClientProvider provider)
    {
        _provider = provider;
    }

    public async Task<bool> SendAsync(
        PointDto command,
        string value,
        CancellationToken token)
    {
        var plcs = await _provider.GetClientsAsync(token);

        var plc = plcs.FirstOrDefault(p => p.Name == command.ControllerName);

        if (plc == null)
            throw new InvalidOperationException($"PLC '{command.ControllerName}' not found.");

        return await plc.WriteAsync(command, double.Parse(value), token);
    }
}
