using System;

namespace BMS.Domain.Events
{
    public record DeviceCommandExecutedDomainEvent(
        string DeviceId,
        string CommandName,
        object? Payload,
        DateTime ExecutedAtUtc
    ) : IDomainEvent;
}
