using BMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Application.Abstraction
{
    public interface IDeviceCommandQueue
    {
        ValueTask EnqueueAsync(DeviceCommand command);
    }
}
