namespace BMS.Domain.Entities.PreventiveMaintenance;

public sealed class PmAttachment : BaseEntity<Guid>
{
    private PmAttachment()
    {
    }

    private PmAttachment(
        Guid? pmScheduleId,
        Guid? pmServiceHistoryId,
        string fileName,
        string contentType,
        string? fileExtension,
        byte[] fileContent,
        string? description,
        Guid? createdByUserId)
    {
        if (pmScheduleId.HasValue == pmServiceHistoryId.HasValue)
            throw new ArgumentException("An attachment must belong to exactly one PM owner.");

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("FileName is required.", nameof(fileName));

        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("ContentType is required.", nameof(contentType));

        if (fileContent.Length == 0)
            throw new ArgumentException("FileContent cannot be empty.", nameof(fileContent));

        PmScheduleId = pmScheduleId;
        PmServiceHistoryId = pmServiceHistoryId;
        FileName = fileName.Trim();
        ContentType = contentType.Trim();
        FileExtension = string.IsNullOrWhiteSpace(fileExtension) ? null : fileExtension.Trim().ToLowerInvariant();
        FileSize = fileContent.LongLength;
        FileContent = fileContent;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        CreatedByUserId = createdByUserId;
    }

    public Guid? PmScheduleId { get; private set; }
    public PmSchedule? PmSchedule { get; private set; }

    public Guid? PmServiceHistoryId { get; private set; }
    public PmServiceHistory? PmServiceHistory { get; private set; }

    public string FileName { get; private set; } = default!;
    public string ContentType { get; private set; } = default!;
    public string? FileExtension { get; private set; }
    public long FileSize { get; private set; }
    public byte[] FileContent { get; private set; } = Array.Empty<byte>();
    public string? Description { get; private set; }
    public Guid? CreatedByUserId { get; private set; }

    public static PmAttachment ForSchedule(
        Guid pmScheduleId,
        string fileName,
        string contentType,
        string? fileExtension,
        byte[] fileContent,
        string? description,
        Guid? createdByUserId)
    {
        if (pmScheduleId == Guid.Empty)
            throw new ArgumentException("PmScheduleId is required.", nameof(pmScheduleId));

        return new PmAttachment(
            pmScheduleId,
            null,
            fileName,
            contentType,
            fileExtension,
            fileContent,
            description,
            createdByUserId);
    }

    public static PmAttachment ForHistory(
        Guid pmServiceHistoryId,
        string fileName,
        string contentType,
        string? fileExtension,
        byte[] fileContent,
        string? description,
        Guid? createdByUserId)
    {
        if (pmServiceHistoryId == Guid.Empty)
            throw new ArgumentException("PmServiceHistoryId is required.", nameof(pmServiceHistoryId));

        return new PmAttachment(
            null,
            pmServiceHistoryId,
            fileName,
            contentType,
            fileExtension,
            fileContent,
            description,
            createdByUserId);
    }
}
