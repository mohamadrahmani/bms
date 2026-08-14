using BMS.Domain.Entities.BMS;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BMS.Application.Common.Interfaces
{
    public interface IDeviceScheduleRepository
    {
        IQueryable<DeviceSchedule> DeviceSchedules { get; }
        
        Task<DeviceSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<List<DeviceSchedule>> GetByDeviceIdAsync(Guid deviceId, CancellationToken cancellationToken);
        Task AddAsync(DeviceSchedule deviceSchedule, CancellationToken cancellationToken = default);
    }
}
