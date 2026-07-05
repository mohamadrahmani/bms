using BMS.Domain.Entities.BMS;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BMS.Application.Common.Interfaces
{
    public interface IDeviceScheduleRepository
    {
        // متد اصلی که در هندلر به آن نیاز دارید
        Task<DeviceSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        // اگر در آینده برای لیست کردن نیاز به کوئری داشتید
        Task<List<DeviceSchedule>> GetByDeviceIdAsync(Guid deviceId, CancellationToken cancellationToken);
    }
}
