using BMS.Domain.Enums;

namespace BMS.Domain.Entities.PreventiveMaintenance;

public sealed class PmServiceHistory : BaseEntity<Guid>
{
    private readonly List<PmAttachment> _attachments = new();

    private PmServiceHistory()
    {
    }

    internal PmServiceHistory(
        Guid pmScheduleId,
        PmFinalStatus status,
        DateTime actionDateUtc,
        string? description,
        Guid? performedByUserId,
        Guid? createdByUserId,
        DateTime dueDateUtcSnapshot,
        string titleSnapshot,
        string? baseDescriptionSnapshot)
    {
        PmScheduleId = pmScheduleId;
        Status = status;
        ActionDateUtc = actionDateUtc;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        PerformedByUserId = performedByUserId;
        CreatedByUserId = createdByUserId;
        DueDateUtcSnapshot = dueDateUtcSnapshot;
        TitleSnapshot = titleSnapshot;
        BaseDescriptionSnapshot = baseDescriptionSnapshot;
    }

    public Guid PmScheduleId { get; private set; }
    public PmSchedule PmSchedule { get; private set; } = default!;

    public PmFinalStatus Status { get; private set; }
    public DateTime ActionDateUtc { get; private set; }
    public string? Description { get; private set; }
    public Guid? PerformedByUserId { get; private set; }
    public Guid? CreatedByUserId { get; private set; }

    public DateTime DueDateUtcSnapshot { get; private set; }
    public string TitleSnapshot { get; private set; } = default!;
    public string? BaseDescriptionSnapshot { get; private set; }

    public IReadOnlyCollection<PmAttachment> Attachments => _attachments.AsReadOnly();
}
