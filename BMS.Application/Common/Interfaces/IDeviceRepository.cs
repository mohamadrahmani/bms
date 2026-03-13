using BMS.Domain.Entities.BMS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BMS.Application.Common.Interfaces
{
    public interface IDeviceRepository
    {
        Task<Device?> GetByIdAsync(Guid id);

        Task<List<Device>> GetByControllerIdAsync(Guid controllerId);

        Task AddAsync(Device device);

        Task UpdateAsync(Device device);

        Task DeleteAsync(Device device);

        Task<bool> ExistsAsync(Guid id);
        Task<List<Device>> GetAllAsync();

        Task SaveChangesAsync();
    }
}
