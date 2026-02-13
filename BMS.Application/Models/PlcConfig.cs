using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Application.Models;
public class PlcConfig
{
    public string Name { get; set; } = default!;
    public string Ip { get; set; } = default!;
    public int Port { get; set; }
    public byte UnitId { get; set; }
}
