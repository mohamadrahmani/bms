using System;

namespace BMS.Domain.Events
{
    public record DeviceCommandExecutedDomainEvent(
        Guid DeviceId,
        string CommandName,
        object? Payload,
        DateTime ExecutedAtUtc
    ) : IDomainEvent;
}
