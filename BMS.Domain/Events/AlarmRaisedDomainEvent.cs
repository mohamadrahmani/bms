using System;

using BMS.Domain.Enums;
using BMS.Domain.Events;

namespace ScadaLite.Domain.Events
{
    public record AlarmRaisedDomainEvent(
        string DeviceId,
        string PointId,
        AlarmSeverity Severity,
        string Message,
        DateTime RaisedAtUtc
    ) : IDomainEvent;
}
