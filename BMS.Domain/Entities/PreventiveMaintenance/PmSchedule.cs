using BMS.Domain.Entities.BMS;
using BMS.Domain.Enums;

namespace BMS.Domain.Entities.PreventiveMaintenance;

public sealed class PmSchedule : BaseEntity<Guid>
{
    private readonly List<PmAttachment> _attachments = new();

    private PmSchedule()
    {
    }

    public PmSchedule(
        Guid deviceId,
        string title,
        string? description,
        DateTime dueDateUtc,
        int warningDays,
        Guid? createdByUserId)
    {
        if (deviceId == Guid.Empty)
            throw new ArgumentException("DeviceId is required.", nameof(deviceId));

        DeviceId = deviceId;
        SetInformation(title, description, dueDateUtc, warningDays);
        CreatedByUserId = createdByUserId;
        IsActive = true;
    }

    public Guid DeviceId { get; private set; }
    public Device Device { get; private set; } = default!;

    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public DateTime DueDateUtc { get; private set; }
    public int WarningDays { get; private set; }
    public bool IsActive { get; private set; }

    public Guid? CreatedByUserId { get; private set; }
    public Guid? UpdatedByUserId { get; private set; }
    public DateTime? ClosedAtUtc { get; private set; }
    public Guid? ClosedByUserId { get; private set; }

    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public PmServiceHistory? ServiceHistory { get; private set; }
    public IReadOnlyCollection<PmAttachment> Attachments => _attachments.AsReadOnly();

    public void Update(
        string title,
        string? description,
        DateTime dueDateUtc,
        int warningDays,
        Guid? updatedByUserId)
    {
        EnsureActive();
        SetInformation(title, description, dueDateUtc, warningDays);
        UpdatedByUserId = updatedByUserId;
        SetUpdated();
    }

    public PmServiceHistory Finalize(
        PmFinalStatus status,
        DateTime actionDateUtc,
        string? description,
        Guid? performedByUserId,
        Guid? closedByUserId,
        DateTime closedAtUtc)
    {
        EnsureActive();

        if (!Enum.IsDefined(status))
            throw new ArgumentOutOfRangeException(nameof(status), "Unsupported PM final status.");

        var normalizedActionDate = NormalizeUtc(actionDateUtc);
        var normalizedClosedAt = NormalizeUtc(closedAtUtc);

        IsActive = false;
        ClosedAtUtc = normalizedClosedAt;
        ClosedByUserId = closedByUserId;
        UpdatedByUserId = closedByUserId;
        SetUpdated();

        var history = new PmServiceHistory(
            Id,
            status,
            normalizedActionDate,
            description,
            performedByUserId,
            closedByUserId,
            DueDateUtc,
            Title,
            Description);

        ServiceHistory = history;
        return history;
    }

    public PmIndicatorStatus CalculateIndicator(DateTime nowUtc)
    {
        if (!IsActive)
            return PmIndicatorStatus.None;

        var normalizedNow = NormalizeUtc(nowUtc);
        if (normalizedNow >= DueDateUtc)
            return PmIndicatorStatus.Overdue;

        var warningStartUtc = DueDateUtc.AddDays(-WarningDays);
        return normalizedNow >= warningStartUtc
            ? PmIndicatorStatus.Warning
            : PmIndicatorStatus.Normal;
    }

    private void SetInformation(
        string title,
        string? description,
        DateTime dueDateUtc,
        int warningDays)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        var normalizedTitle = title.Trim();
        if (normalizedTitle.Length > 200)
            throw new ArgumentException("Title cannot exceed 200 characters.", nameof(title));

        var normalizedDescription = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();

        if (normalizedDescription?.Length > 2000)
            throw new ArgumentException("Description cannot exceed 2000 characters.", nameof(description));

        if (warningDays < 0)
            throw new ArgumentOutOfRangeException(nameof(warningDays), "WarningDays cannot be negative.");

        Title = normalizedTitle;
        Description = normalizedDescription;
        DueDateUtc = NormalizeUtc(dueDateUtc);
        WarningDays = warningDays;
    }

    private void EnsureActive()
    {
        if (!IsActive)
            throw new InvalidOperationException("Closed PM schedules cannot be changed.");
    }

    private static DateTime NormalizeUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}
