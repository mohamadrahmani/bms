using BMS.Domain.Entities;
using BMS.Domain.Entities.BMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Application.Models;
public class PlcConfig : Controller
{
    //public string Name { get; set; } = default!;
    //public string Ip { get; set; } = default!;
    //public int Port { get; set; }

    public List<DeviceConfig> Devices { get; set; } = new();
}
