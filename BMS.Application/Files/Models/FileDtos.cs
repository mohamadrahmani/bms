namespace BMS.Application.Files.Models;

public sealed record FileValidationRequest(
    Guid EntityTypeId,
    string FileName,
    long FileSize,
    string ContentType,
    string? FileExtension);

public sealed record FileValidationResult(
    bool IsValid,
    string? Code = null,
    string? Message = null);

public sealed record FileDto(
    Guid Id,
    Guid? AttachmentId,
    string OriginalFileName,
    string ContentType,
    string? FileExtension,
    long FileSize,
    string? Description,
    DateTime CreatedAtUtc);

public sealed record FileDownload(
    string FileName,
    string ContentType,
    Stream Content);
