using System;

namespace BMS.Domain.Events
{
    public record DeviceCommandExecutedDomainEvent(
        Guid DeviceId,
        string CommandName,
        string? value,
        DateTime ExecutedAtUtc
    ) : IDomainEvent;
}
