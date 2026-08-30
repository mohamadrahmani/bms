using System.Text;

namespace BMS.Application.Files;

public static class FileValidationRules
{
    public static (bool Valid, string? Code, string? Message) ValidateMetadata(
        string? fileName,
        long fileSize,
        string? contentType,
        string? extension,
        BMS.Domain.Entities.Files.FileEntityTypeConfiguration configuration)
    {
        if (!configuration.IsUploadAllowed)
            return Invalid("FILE_UPLOAD_NOT_ALLOWED", "آپلود فایل برای این نوع موجودیت مجاز نیست.");
        if (fileSize == 0)
            return Invalid("FILE_EMPTY", "امکان ثبت فایل خالی وجود ندارد.");
        if (fileSize < 0 || fileSize > (long)configuration.MaxFileSize * 1024)
            return Invalid("FILE_TOO_LARGE", "حجم فایل از حد مجاز بیشتر است.");

        var normalizedExtension = NormalizeExtension(extension ?? Path.GetExtension(fileName));
        var allowedExtensions = Split(configuration.AllowedExtensions)
            .Select(NormalizeExtension)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(normalizedExtension) ||
            !allowedExtensions.Contains(normalizedExtension, StringComparer.OrdinalIgnoreCase))
            return Invalid("FILE_EXTENSION_NOT_ALLOWED", "فرمت فایل مجاز نیست.");

        if (string.IsNullOrWhiteSpace(fileName) ||
            fileName.Length > 255 ||
            fileName != Path.GetFileName(fileName) ||
            fileName.Contains("..", StringComparison.Ordinal) ||
            fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            return Invalid("FILE_NAME_INVALID", "نام فایل معتبر نیست.");

        var allowedContentTypes = Split(configuration.AllowedContentTypes);
        if (!allowedContentTypes.Contains("*", StringComparer.OrdinalIgnoreCase) &&
            !allowedContentTypes.Contains(contentType?.Trim() ?? "", StringComparer.OrdinalIgnoreCase))
            return Invalid("FILE_CONTENT_TYPE_NOT_ALLOWED", "نوع محتوای فایل مجاز نیست.");

        return (true, null, null);
    }

    public static bool HasValidSignature(string extension, byte[] content)
    {
        var ext = NormalizeExtension(extension);
        if (content.Length == 0)
            return true;

        return ext switch
        {
            ".png" => StartsWith(content, 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A),
            ".jpg" or ".jpeg" => StartsWith(content, 0xFF, 0xD8, 0xFF),
            ".gif" => StartsWithAscii(content, "GIF87a") ||
                      StartsWithAscii(content, "GIF89a"),
            ".bmp" => StartsWith(content, 0x42, 0x4D),
            ".webp" => StartsWithAscii(content, "RIFF") &&
                       StartsWithAscii(content, "WEBP", 8),
            ".pdf" => StartsWithAscii(content, "%PDF-"),
            ".zip" or ".docx" or ".xlsx" or ".pptx" or ".odt" or ".ods" or ".odp" =>
                StartsWith(content, 0x50, 0x4B, 0x03, 0x04) ||
                StartsWith(content, 0x50, 0x4B, 0x05, 0x06) ||
                StartsWith(content, 0x50, 0x4B, 0x07, 0x08),
            ".rar" => StartsWithAscii(content, "Rar!\x1A\x07\x00") ||
                      StartsWithAscii(content, "Rar!\x1A\x07\x01\x00"),
            ".7z" => StartsWith(content, 0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C),
            ".gz" => StartsWith(content, 0x1F, 0x8B),
            ".mp3" => StartsWithAscii(content, "ID3") ||
                      (content.Length >= 2 && content[0] == 0xFF &&
                       (content[1] & 0xE0) == 0xE0),
            ".wav" => StartsWithAscii(content, "RIFF") &&
                      StartsWithAscii(content, "WAVE", 8),
            ".flac" => StartsWithAscii(content, "fLaC"),
            ".ogg" or ".oga" or ".ogv" => StartsWithAscii(content, "OggS"),
            ".mp4" or ".m4v" or ".mov" => HasIsoBaseMediaSignature(content),
            ".avi" => StartsWithAscii(content, "RIFF") &&
                      StartsWithAscii(content, "AVI ", 8),
            ".html" or ".htm" or ".xml" or ".json" or ".csv" or ".log" or ".md" or ".txt" =>
                IsTextContent(content),
            _ => true
        };
    }

    private static bool StartsWith(byte[] content, params byte[] signature) =>
        content.AsSpan().StartsWith(signature);

    private static bool StartsWithAscii(
        byte[] content, string signature, int offset = 0)
    {
        if (offset < 0 || content.Length < offset + signature.Length)
            return false;

        for (var index = 0; index < signature.Length; index++)
        {
            if (content[offset + index] != (byte)signature[index])
                return false;
        }

        return true;
    }

    private static bool IsTextContent(byte[] content)
    {
        if (content.Contains((byte)0))
            return false;

        try
        {
            _ = Encoding.UTF8.GetString(content);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool HasIsoBaseMediaSignature(byte[] content) =>
        content.Length >= 12 && StartsWithAscii(content, "ftyp", 4);

    public static string NormalizeExtension(string? extension)
    {
        var value = (extension ?? "").Trim().ToLowerInvariant();
        return value.StartsWith('.') ? value : $".{value}";
    }

    private static HashSet<string> Split(string value) =>
        value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

    private static (bool, string, string) Invalid(string code, string message) =>
        (false, code, message);
}
