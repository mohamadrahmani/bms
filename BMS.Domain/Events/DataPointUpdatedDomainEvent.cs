using System;

namespace BMS.Domain.Events
{
    public record DataPointUpdatedDomainEvent(
        string DeviceId,
        string PointId,
        object? Value,
        DateTime TimestampUtc
    ) : IDomainEvent;
}
