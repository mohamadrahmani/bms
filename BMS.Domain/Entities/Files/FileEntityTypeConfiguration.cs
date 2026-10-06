using BMS.Domain.Entities;

namespace BMS.Domain.Entities.Files;

public class FileEntityTypeConfiguration : BaseEntity<Guid>
{
    private FileEntityTypeConfiguration()
    {
    }

    public FileEntityTypeConfiguration(
        Guid entityTypeId,
        bool isUploadAllowed,
        int maxFileSize,
        int maxFileCount,
        string allowedExtensions,
        string allowedContentTypes)
    {
        EntityTypeId = entityTypeId;
        IsUploadAllowed = isUploadAllowed;
        MaxFileSize = maxFileSize;
        MaxFileCount = maxFileCount;
        AllowedExtensions = allowedExtensions;
        AllowedContentTypes = allowedContentTypes;
    }

    public Guid EntityTypeId { get; private set; }
    public bool IsUploadAllowed { get; private set; }
    public int MaxFileSize { get; private set; }
    public int MaxFileCount { get; private set; }
    public string AllowedExtensions { get; private set; } = null!;
    public string AllowedContentTypes { get; private set; } = null!;

    public EntityType EntityType { get; private set; } = null!;
}
