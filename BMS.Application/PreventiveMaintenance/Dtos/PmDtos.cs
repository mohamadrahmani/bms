using BMS.Domain.Enums;

namespace BMS.Application.PreventiveMaintenance.Dtos;

public sealed class ActivePmDto
{
    public bool HasActivePm { get; init; }
    public Guid? PmScheduleId { get; init; }
    public Guid DeviceId { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public DateTime? DueDateUtc { get; init; }
    public int? WarningDays { get; init; }
    public PmIndicatorStatus Indicator { get; init; }
    public int? DaysUntilDue { get; init; }
    public byte[]? RowVersion { get; init; }
    public IReadOnlyList<PmAttachmentDto> Attachments { get; init; } = Array.Empty<PmAttachmentDto>();
}

public sealed class PmHistoryDto
{
    public Guid Id { get; init; }
    public Guid PmScheduleId { get; init; }
    public PmFinalStatus Status { get; init; }
    public DateTime ActionDateUtc { get; init; }
    public string? Description { get; init; }
    public Guid? PerformedByUserId { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public Guid? CreatedByUserId { get; init; }
    public DateTime DueDateUtcSnapshot { get; init; }
    public string TitleSnapshot { get; init; } = default!;
    public string? BaseDescriptionSnapshot { get; init; }
    public IReadOnlyList<PmAttachmentDto> Attachments { get; init; } = Array.Empty<PmAttachmentDto>();
}

public sealed class PmAttachmentDto
{
    public Guid Id { get; init; }
    public string FileName { get; init; } = default!;
    public string ContentType { get; init; } = default!;
    public string? FileExtension { get; init; }
    public long FileSize { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public Guid? CreatedByUserId { get; init; }
}

public sealed class PmAttachmentDownloadDto
{
    public string FileName { get; init; } = default!;
    public string ContentType { get; init; } = default!;
    public byte[] Content { get; init; } = Array.Empty<byte>();
}
