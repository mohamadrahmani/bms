using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Domain.Entities.BMS
{
    public class DeviceCommand
    {
        public string CommandId { get; }
        public string DeviceId { get; }
        public string CommandName { get; }
        public object? Payload { get; }
        public DateTime CreatedAtUtc { get; }

        public DeviceCommand(
            string deviceId,
            string commandName,
            object? payload)
        {
            CommandId = Guid.NewGuid().ToString();
            DeviceId = deviceId;
            CommandName = commandName;
            Payload = payload;
            CreatedAtUtc = DateTime.UtcNow;
        }
    }
}
