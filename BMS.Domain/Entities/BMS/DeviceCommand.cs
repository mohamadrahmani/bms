using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Domain.Entities.BMS;
public class DeviceCommand : BaseEntity<int>
{
    public Guid? DeviceId { get; set; }

    public Guid CommandDefinitionId { get; set; }
    public string CommandName { get; set; }

    public Guid? PointId { get; set; }

    public string ParameterValue { get; set; }
    public string? Value { get; set; }

    public CommandStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ExecutedAt { get; set; }

    public string ResultMessage { get; set; }
    //public DeviceCommand(
    //        Guid deviceId,
    //        string commandName,
    //        object? payload)
    //{
    //    //CommandId = Guid.NewGuid().ToString();
    //    DeviceId = deviceId;
    //    //CommandName = commandName;
    //    //Payload = payload;
    //    CreatedAtUtc = DateTime.UtcNow;
    //}
}

public enum CommandStatus
{
    Pending = 0,
    Sent = 1,
    Success = 2,
    Failed = 3
}