using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Infrastructure.Modbus;

public enum ConnectionState
{
    Unknown = 0,
    Online = 1,
    Offline = 2,
    HalfOpen = 3
}

