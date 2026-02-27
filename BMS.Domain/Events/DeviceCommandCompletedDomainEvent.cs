namespace BMS.Domain.Events;

public record DeviceCommandCompletedDomainEvent(
    string CommandId,
    string DeviceId,
    string CommandName,
    bool Success,
    string? Message,
    DateTime CompletedAtUtc
) : IDomainEvent;