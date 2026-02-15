using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Application.Models;
public class DeviceConfig
{
    public Guid DeviceId { get; set; }

    public byte SlaveId { get; set; }

    public ushort StartAddress { get; set; }

    public ushort RegisterCount { get; set; }

    public List<SensorConfig> Sensors { get; set; } = new();
}

