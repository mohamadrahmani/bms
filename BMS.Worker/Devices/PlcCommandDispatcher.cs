using BMS.Application.Abstraction;
using BMS.Application.Models;
using BMS.Worker.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Worker.Devices;
public class PlcCommandDispatcher : IPlcCommandDispatcher
{
    private readonly IEnumerable<IPlcClient> _plcs;

    public PlcCommandDispatcher(IEnumerable<IPlcClient> plcs)
    {
        _plcs = plcs;
    }

    public async Task<bool> SendAsync(
        WritePointCommand command,
        CancellationToken token)
    {
        var plc = _plcs.FirstOrDefault(p => p.Name == command.PlcName);

        if (plc == null)
            throw new InvalidOperationException("PLC not found.");

         await plc.WriteAsync(command, token); 
        return true;

    }

}

