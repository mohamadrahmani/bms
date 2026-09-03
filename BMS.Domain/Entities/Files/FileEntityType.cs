using BMS.Domain.Entities;

namespace BMS.Domain.Entities.Files;

public class FileEntityType : BaseEntity<Guid>
{
    private FileEntityType()
    {
    }

    public FileEntityType(string code, string name)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Entity type code is required.", nameof(code));

        Code = code.Trim();
        Name = string.IsNullOrWhiteSpace(name) ? Code : name.Trim();
        IsActive = true;
    }

    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? TableName { get; private set; }
    public string? DisplayNameFa { get; private set; }
    public string? DisplayNameEn { get; private set; }
    public bool IsActive { get; private set; }

    public FileEntityTypeConfiguration? Configuration { get; private set; }
}
