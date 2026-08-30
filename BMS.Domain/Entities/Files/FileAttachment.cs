using BMS.Domain.Entities;

namespace BMS.Domain.Entities.Files;

public class FileAttachment : BaseEntity<Guid>
{
    private FileAttachment()
    {
    }

    public FileAttachment(Guid fileId, Guid entityTypeId, Guid entityId)
    {
        FileId = fileId;
        EntityTypeId = entityTypeId;
        EntityId = entityId;
    }

    public Guid FileId { get; private set; }
    public Guid EntityTypeId { get; private set; }
    public Guid EntityId { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public Guid? DeletedByUserId { get; private set; }

    public File File { get; private set; } = null!;

    public void MarkDeleted(Guid? deletedByUserId = null)
    {
        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
        DeletedByUserId = deletedByUserId;
    }
}
