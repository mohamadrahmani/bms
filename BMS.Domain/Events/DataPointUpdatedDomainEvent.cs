using System;

namespace BMS.Domain.Events
{
    public record DataPointUpdatedDomainEvent(
        Guid DeviceId,
        Guid PointId,
        object? Value,
        DateTime TimestampUtc
    ) : IDomainEvent;
}
