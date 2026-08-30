using BMS.Domain.Entities;

namespace BMS.Domain.Entities.Files;

public class File : BaseEntity<Guid>
{
    private File()
    {
    }

    public File(
        string originalFileName,
        string contentType,
        string? fileExtension,
        long fileSize,
        string? checksum,
        string? description)
    {
        OriginalFileName = originalFileName.Trim();
        ContentType = contentType.Trim();
        FileExtension = NormalizeExtension(fileExtension);
        FileSize = (long)Math.Ceiling(fileSize / 1024d);
        Checksum = checksum;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    public string OriginalFileName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public string? FileExtension { get; private set; }
    public long FileSize { get; private set; }
    public string? Checksum { get; private set; }
    public string? Description { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public Guid? DeletedByUserId { get; private set; }

    public FileContent? Content { get; private set; }
    public ICollection<FileAttachment> Attachments { get; private set; } = new List<FileAttachment>();

    public void MarkDeleted(Guid? deletedByUserId = null)
    {
        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
        DeletedByUserId = deletedByUserId;
    }

    public void SetContent(FileContent content) => Content = content;

    public void SetDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
    }

    private static string? NormalizeExtension(string? extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
            return null;
        var value = extension.Trim().ToLowerInvariant();
        return value.StartsWith('.') ? value : $".{value}";
    }
}
