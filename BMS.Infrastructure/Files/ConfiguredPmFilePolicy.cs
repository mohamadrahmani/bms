using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Settings;
using Microsoft.Extensions.Options;

namespace BMS.Infrastructure.Files;

public sealed class ConfiguredPmFilePolicy : IPmFilePolicy
{
    private readonly PmFileOptions _options;
    private readonly HashSet<string> _allowedExtensions;
    private readonly HashSet<string> _allowedContentTypes;

    public ConfiguredPmFilePolicy(IOptions<PmFileOptions> options)
    {
        _options = options.Value;
        _allowedExtensions = _options.AllowedExtensions
            .Select(NormalizeExtension)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        _allowedContentTypes = _options.AllowedContentTypes
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public string? Validate(string fileName, string contentType, long fileSize)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return "File name is required.";

        if (Path.GetFileName(fileName) != fileName)
            return "File name cannot contain a path.";

        if (fileSize <= 0)
            return "File cannot be empty.";

        if (fileSize > _options.MaxFileSizeBytes)
            return $"File size cannot exceed {_options.MaxFileSizeBytes} bytes.";

        var extension = NormalizeExtension(Path.GetExtension(fileName));
        if (string.IsNullOrWhiteSpace(extension) || !_allowedExtensions.Contains(extension))
            return "File extension is not allowed.";

        if (string.IsNullOrWhiteSpace(contentType) || !_allowedContentTypes.Contains(contentType))
            return "File content type is not allowed.";

        return null;
    }

    private static string NormalizeExtension(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
            return string.Empty;

        var value = extension.Trim().ToLowerInvariant();
        return value.StartsWith('.') ? value : $".{value}";
    }
}
