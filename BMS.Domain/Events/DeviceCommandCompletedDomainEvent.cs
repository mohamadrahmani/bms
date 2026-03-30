namespace BMS.Domain.Events;

public record DeviceCommandCompletedDomainEvent(
    Guid CommandId,
    Guid? DeviceId,
    Guid PointId,
    string CommandName,
    bool Success,
    string? Message,
    DateTime CompletedAtUtc
) : IDomainEvent;