using BMS.Application.Abstraction;
using BMS.Domain.Entities.BMS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using BMS.Application.Common.Interfaces;

namespace BMS.Infrastructure.Persistence.Repositories
{
    public class DeviceScheduleRepository : IDeviceScheduleRepository
    {
        private readonly BMSDbContext _context; // نام کلاس DbContext شما
        public IQueryable<DeviceSchedule> DeviceSchedules => _context.DeviceSchedules;

        public DeviceScheduleRepository(BMSDbContext context)
        {
            _context = context;
        }

        public async Task<DeviceSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.DeviceSchedules
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        }

        public async Task<List<DeviceSchedule>> GetByDeviceIdAsync(Guid deviceId, CancellationToken cancellationToken)
        {
            return await _context.DeviceSchedules
                .Where(x => x.DeviceId == deviceId && !x.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(DeviceSchedule deviceSchedule, CancellationToken cancellationToken = default)
        {
            await _context.DeviceSchedules.AddAsync(deviceSchedule, cancellationToken);
        }
    }
}
