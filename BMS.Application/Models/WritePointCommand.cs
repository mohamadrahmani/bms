using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Application.Models;
public class WritePointCommand
{
    public string PlcName { get; set; } = default!;
    public byte SlaveId { get; set; }
    public ushort Address { get; set; }
    public ushort Value { get; set; }
}

