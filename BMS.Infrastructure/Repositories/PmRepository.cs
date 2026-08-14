using BMS.Application.Common.Interfaces;
using BMS.Domain.Entities.PreventiveMaintenance;
using BMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BMS.Infrastructure.Persistence.Repositories;

public sealed class PmRepository : IPmRepository
{
    private readonly BMSDbContext _context;

    public PmRepository(BMSDbContext context)
    {
        _context = context;
    }

    public IQueryable<PmSchedule> Schedules => _context.PmSchedules;
    public IQueryable<PmServiceHistory> Histories => _context.PmServiceHistories;
    public IQueryable<PmAttachment> Attachments => _context.PmAttachments;

    public Task<PmSchedule?> GetScheduleByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.PmSchedules
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public Task<PmServiceHistory?> GetHistoryByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.PmServiceHistories
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public Task<PmAttachment?> GetAttachmentByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.PmAttachments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public Task<bool> HasActiveScheduleAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        return _context.PmSchedules.AnyAsync(
            x => x.DeviceId == deviceId && x.IsActive && !x.IsDeleted,
            cancellationToken);
    }

    public Task AddScheduleAsync(PmSchedule schedule, CancellationToken cancellationToken)
    {
        return _context.PmSchedules.AddAsync(schedule, cancellationToken).AsTask();
    }

    public Task AddHistoryAsync(PmServiceHistory history, CancellationToken cancellationToken)
    {
        return _context.PmServiceHistories.AddAsync(history, cancellationToken).AsTask();
    }

    public Task AddAttachmentAsync(PmAttachment attachment, CancellationToken cancellationToken)
    {
        return _context.PmAttachments.AddAsync(attachment, cancellationToken).AsTask();
    }
}
