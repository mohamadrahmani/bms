using BMS.Domain.Entities.BMS;
using BMS.Domain.Enums;

namespace BMS.Application.Common.Interfaces
{
    public interface IPmRepository
    {
        IQueryable<PmSchedule> PmSchedules { get; }

        IQueryable<PmServiceHistory> PmServiceHistories { get; }

        Task<PmSchedule?> GetByIdAsync(Guid id);

        Task<PmSchedule?> GetActiveByDeviceIdAsync(Guid deviceId);

        Task<bool> HasActivePmAsync(Guid deviceId, string? tag);

        Task AddAsync(PmSchedule pmSchedule);

        Task UpdateAsync(PmSchedule pmSchedule);

        Task AddHistoryAsync(PmServiceHistory history);

        Task<List<PmServiceHistory>> GetHistoryByDeviceIdAsync(
            Guid deviceId, string tag);

        Task<PmServiceHistory?> GetHistoryByIdAsync(Guid id);

        Task SaveChangesAsync();

        Task<Guid?> FinalizeAsync(
            Guid pmScheduleId,
            PmServiceStatus status,
            DateTime actionDateUtc,
            string? description,
            //Guid? performedByUserId,
            Guid? closedByUserId,
            Guid? createdByUserId);
    }
}
