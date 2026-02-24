using BMS.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Worker.Abstractions;
public interface IPlcCommandDispatcher
{
    Task<bool> SendAsync(WritePointCommand command, CancellationToken token);
}

