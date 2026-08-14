using BMS.Application.Attributes;
using BMS.Domain.Entities.Logs;
using BMS.Domain.Enums;
using MediatR;

namespace BMS.Application.PreventiveMaintenance.Commands;

[Audit(EventType.AddData, "PreventiveMaintenance")]
public sealed class CreatePmScheduleCommand : IRequest<Guid>
{
    public Guid DeviceId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime DueDateUtc { get; set; }
    public int WarningDays { get; set; }
}

[Audit(EventType.UpdateData, "PreventiveMaintenance")]
public sealed class UpdatePmScheduleCommand : IRequest
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime DueDateUtc { get; set; }
    public int WarningDays { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}

[Audit(EventType.UpdateData, "PreventiveMaintenance")]
public sealed class FinalizePmScheduleCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
    public PmFinalStatus Status { get; set; }
    public DateTime ActionDateUtc { get; set; }
    public string? Description { get; set; }
    public Guid? PerformedByUserId { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
