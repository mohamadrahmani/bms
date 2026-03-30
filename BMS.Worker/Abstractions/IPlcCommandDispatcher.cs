using BMS.Application.Models;
using BMS.Application.Points.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Worker.Abstractions;
public interface IPlcCommandDispatcher
{
    Task<bool> SendAsync(PointDto command,string value, CancellationToken token);
}

