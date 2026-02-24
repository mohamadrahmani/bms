using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Application.Models;
public class WritePointCommand
{
    public string PlcName { get; set; } = default!;
    public string DeviceName { get; set; } = default!;
    public Guid DeviceId { get; set; }
    public string PointCode { get; set; } = default!;
    public double Value { get; set; }
}


