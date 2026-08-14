using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Domain.Entities.BMS
{
    public class SensorCommand
    {
        public Guid? DeviceId { get; set; }
        public string CommandName { get; set; }
        public string? Value { get; set; }
    }
}
