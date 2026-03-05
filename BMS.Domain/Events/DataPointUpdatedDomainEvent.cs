using System;

namespace BMS.Domain.Events
{
    public record DataPointUpdatedDomainEvent(
        Guid DeviceId,
        Guid Id,
        object? Value,
        DateTime TimestampUtc
    ) : IDomainEvent;
}
