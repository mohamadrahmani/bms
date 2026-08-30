using BMS.Domain.Entities;

namespace BMS.Domain.Entities.Files;

public class FileContent : BaseEntity<Guid>
{
    private FileContent()
    {
    }

    public FileContent(Guid fileId, byte[] content)
    {
        FileId = fileId;
        Content = content;
    }

    public Guid FileId { get; private set; }
    public byte[]? Content { get; private set; }
    public File File { get; private set; } = null!;
}
