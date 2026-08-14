using BMS.Domain.Entities.PreventiveMaintenance;

namespace BMS.Application.Common.Interfaces;

public interface IPmRepository
{
    IQueryable<PmSchedule> Schedules { get; }
    IQueryable<PmServiceHistory> Histories { get; }
    IQueryable<PmAttachment> Attachments { get; }

    Task<PmSchedule?> GetScheduleByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PmServiceHistory?> GetHistoryByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PmAttachment?> GetAttachmentByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> HasActiveScheduleAsync(Guid deviceId, CancellationToken cancellationToken);

    Task AddScheduleAsync(PmSchedule schedule, CancellationToken cancellationToken);
    Task AddHistoryAsync(PmServiceHistory history, CancellationToken cancellationToken);
    Task AddAttachmentAsync(PmAttachment attachment, CancellationToken cancellationToken);
}
