using BMS.Application.Common.Interfaces;
using BMS.Domain.Entities.BMS;
using BMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BMS.Infrastructure.Persistence.Repositories
{
    public class PmRepository : IPmRepository
    {
        private readonly BMSDbContext _context;

        public PmRepository(BMSDbContext context)
        {
            _context = context;
        }

        public IQueryable<PmSchedule> PmSchedules =>
            _context.PmSchedules;

        public IQueryable<PmServiceHistory> PmServiceHistories =>
            _context.PmServiceHistories;


        public async Task<PmSchedule?> GetByIdAsync(Guid id)
        {
            return await _context.PmSchedules
                .FirstOrDefaultAsync(x => x.Id == id);
        }


        public async Task<PmSchedule?> GetActiveByDeviceIdAsync(
            Guid deviceId)
        {
            return await _context.PmSchedules
                .FirstOrDefaultAsync(x =>
                    x.DeviceId == deviceId &&
                    x.IsActive);
        }


        public async Task<bool> HasActivePmAsync(
            Guid deviceId, string? Tag)
        {
            return await _context.PmSchedules
                .AnyAsync(x =>
                    x.DeviceId == deviceId &&
                    (string.IsNullOrEmpty(Tag) || x.Tag == Tag) &&
                    x.IsActive);
        }

        public async Task<List<PmServiceHistory>> GetHistoryByDeviceIdAsync(
    Guid deviceId, string tag)
        {
            return await _context.PmServiceHistories
                .AsNoTracking()
                .Where(x =>
                    x.PmSchedule.DeviceId == deviceId && x.PmSchedule.Tag == tag)
                .OrderByDescending(x => x.ActionDateUtc)
                .ToListAsync();
        }

        public async Task<bool> FinalizeAsync(
            Guid pmScheduleId,
            PmServiceStatus status,
            DateTime actionDateUtc,
            string? description,
            //Guid? performedByUserId,
            Guid? closedByUserId,
            Guid? createdByUserId)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    System.Data.IsolationLevel.Serializable);

            try
            {
                var pm = await _context.PmSchedules
                    .FirstOrDefaultAsync(x =>
                        x.Id == pmScheduleId &&
                        x.IsActive);

                if (pm == null)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                var history = new PmServiceHistory(
                    pm.Id,
                    status,
                    actionDateUtc,
                    description,
                    //performedByUserId,
                    pm.DueDate,
                    pm.Title,
                    pm.Description,
                    createdByUserId);

                await _context.PmServiceHistories.AddAsync(history);

                // pm.Close(closedByUserId);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task AddAsync(
            PmSchedule pmSchedule)
        {
            await _context.PmSchedules.AddAsync(pmSchedule);
        }


        public async Task UpdateAsync(
            PmSchedule pmSchedule)
        {
            _context.PmSchedules.Update(pmSchedule);

            await Task.CompletedTask;
        }


        public async Task AddHistoryAsync(
            PmServiceHistory history)
        {
            await _context.PmServiceHistories.AddAsync(history);
        }


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}