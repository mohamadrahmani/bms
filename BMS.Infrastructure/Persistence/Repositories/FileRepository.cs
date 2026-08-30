using BMS.Application.Common.Interfaces;
using BMS.Domain.Entities.Files;
using Microsoft.EntityFrameworkCore;
using FileEntity = BMS.Domain.Entities.Files.File;

namespace BMS.Infrastructure.Persistence.Repositories;

public sealed class FileRepository : IFileRepository
{
    private readonly BMSDbContext _context;

    public FileRepository(BMSDbContext context) => _context = context;

    public Task<FileEntityType?> GetTypeWithConfigurationAsync(
        Guid entityTypeId, CancellationToken cancellationToken) =>
        _context.Set<FileEntityType>()
            .Include(x => x.Configuration)
            .SingleOrDefaultAsync(x => x.Id == entityTypeId && x.IsActive, cancellationToken);

    public Task<int> CountActiveAttachmentsAsync(
        Guid entityTypeId, Guid entityId, CancellationToken cancellationToken) =>
        _context.Set<FileAttachment>()
            .CountAsync(x => x.EntityTypeId == entityTypeId &&
                             x.EntityId == entityId &&
                             !x.IsDeleted &&
                             !x.File.IsDeleted, cancellationToken);

    public Task<bool> HasActiveAttachmentAsync(
        Guid fileId, Guid entityTypeId, Guid entityId, CancellationToken cancellationToken) =>
        _context.Set<FileAttachment>()
            .AnyAsync(x => x.FileId == fileId &&
                           x.EntityTypeId == entityTypeId &&
                           x.EntityId == entityId &&
                           !x.IsDeleted, cancellationToken);

    public async Task AddAsync(
        FileEntity file, FileContent content, FileAttachment attachment,
        CancellationToken cancellationToken)
    {
        await _context.Set<FileEntity>().AddAsync(file, cancellationToken);
        await _context.Set<FileContent>().AddAsync(content, cancellationToken);
        await _context.Set<FileAttachment>().AddAsync(attachment, cancellationToken);
    }

    public async Task<IReadOnlyList<FileListItem>> GetEntityFilesAsync(
        Guid entityTypeId, Guid entityId, CancellationToken cancellationToken)
    {
        return await _context.Set<FileAttachment>()
            .AsNoTracking()
            .Where(x => x.EntityTypeId == entityTypeId &&
                        x.EntityId == entityId &&
                        !x.IsDeleted &&
                        !x.File.IsDeleted)
            .Select(x => new FileListItem(
                x.File.Id,
                x.Id,
                x.File.OriginalFileName,
                x.File.ContentType,
                x.File.FileExtension,
                x.File.FileSize,
                x.File.Description,
                x.File.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<FileDownloadItem?> GetDownloadAsync(
        Guid fileId, CancellationToken cancellationToken)
    {
        var file = await _context.Set<FileEntity>()
            .AsNoTracking()
            .Include(x => x.Content)
            .SingleOrDefaultAsync(x => x.Id == fileId && !x.IsDeleted, cancellationToken);

        if (file?.Content?.Content is null)
            return null;

        return new FileDownloadItem(
            file.OriginalFileName,
            file.ContentType,
            file.Content.Content);
    }

    public async Task<bool> DeleteAttachmentAsync(
        Guid attachmentId, CancellationToken cancellationToken)
    {
        var attachment = await _context.Set<FileAttachment>()
            .Include(x => x.File)
            .ThenInclude(x => x.Content)
            .SingleOrDefaultAsync(x => x.Id == attachmentId, cancellationToken);

        if (attachment is null || attachment.IsDeleted)
            return true;

        attachment.MarkDeleted();
        var hasOtherActive = await _context.Set<FileAttachment>()
            .AnyAsync(x => x.FileId == attachment.FileId && !x.IsDeleted &&
                           x.Id != attachment.Id, cancellationToken);

        if (!hasOtherActive)
        {
            attachment.File.MarkDeleted();

            if (attachment.File.Content is not null)
            {
                _context.Set<FileContent>().Remove(attachment.File.Content);
            }
        }

        return true;
    }

    public async Task<bool> UpdateDescriptionAsync(
        Guid fileId, string? description, CancellationToken cancellationToken)
    {
        var file = await _context.Set<FileEntity>()
            .SingleOrDefaultAsync(x => x.Id == fileId && !x.IsDeleted, cancellationToken);

        if (file is null)
            return false;

        file.SetDescription(description);
        return true;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        _context.SaveChangesAsync(cancellationToken);

    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<Task<T>> action, CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await action();
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
