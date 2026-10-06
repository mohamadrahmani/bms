using BMS.Domain.Entities.Files;
using FileEntity = BMS.Domain.Entities.Files.File;

namespace BMS.Application.Common.Interfaces;

public interface IFileRepository
{
    Task<EntityType?> GetTypeWithConfigurationAsync(Guid entityTypeId, CancellationToken cancellationToken);
    Task<int> CountActiveAttachmentsAsync(Guid entityTypeId, Guid entityId, CancellationToken cancellationToken);
    Task<bool> HasActiveAttachmentAsync(Guid fileId, Guid entityTypeId, Guid entityId, CancellationToken cancellationToken);
    Task AddAsync(FileEntity file, FileContent content, FileAttachment attachment, CancellationToken cancellationToken);
    Task<IReadOnlyList<FileListItem>> GetEntityFilesAsync(Guid entityTypeId, Guid entityId, CancellationToken cancellationToken);
    Task<FileDownloadItem?> GetDownloadAsync(Guid fileId, CancellationToken cancellationToken);
    Task<bool> DeleteAttachmentAsync(Guid attachmentId, CancellationToken cancellationToken);
    Task<bool> UpdateDescriptionAsync(Guid fileId, string? description, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken);
}

public sealed record FileListItem(
    Guid Id,
    Guid AttachmentId,
    string OriginalFileName,
    string ContentType,
    string? FileExtension,
    long FileSize,
    string? Description,
    DateTime CreatedAtUtc);

public sealed record FileDownloadItem(
    string FileName,
    string ContentType,
    byte[] Content);
