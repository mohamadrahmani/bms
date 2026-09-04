using BMS.Application.Common.Interfaces;
using BMS.Domain.Entities.Logs;
using BMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BMS.Infrastructure.Repositories;

public sealed class SystemErrorLogRepository : ISystemErrorLogRepository
{
    private readonly BMSDbContext _context;

    public SystemErrorLogRepository(BMSDbContext context)
    {
        _context = context;
    }

    public IQueryable<SystemErrorLog> Logs => _context.Set<SystemErrorLog>();

    public async Task<SystemErrorLog?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var log = await Logs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (log?.UserId is Guid userId)
        {
            log.UserName = await _context.Users
                .AsNoTracking()
                .Where(x => x.Id == userId)
                .Select(x => x.UserName)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return log;
    }

    public async Task AddAsync(
        SystemErrorLog log,
        CancellationToken cancellationToken = default)
    {
        await _context.Set<SystemErrorLog>().AddAsync(log, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ResolveAsync(
        Guid id,
        Guid? resolvedByUserId,
        string? resolutionNote,
        CancellationToken cancellationToken = default)
    {
        var log = await _context.Set<SystemErrorLog>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (log is null)
            return false;

        log.IsResolved = true;
        log.ResolvedAtUtc = DateTime.UtcNow;
        log.ResolvedByUserId = resolvedByUserId;
        log.ResolutionNote = string.IsNullOrWhiteSpace(resolutionNote)
            ? null
            : resolutionNote.Trim();

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
