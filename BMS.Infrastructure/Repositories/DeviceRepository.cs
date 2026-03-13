using BMS.Application.Common.Interfaces;
using BMS.Domain.Entities.BMS;
using BMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BMS.Infrastructure.Persistence.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly BMSDbContext _context;

        public DeviceRepository(BMSDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Device device)
        {
            await _context.Devices.AddAsync(device);
        }

        public async Task DeleteAsync(Device device)
        {
            _context.Devices.Remove(device);
            await Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Devices.AnyAsync(x => x.Id == id);
        }

        public async Task<Device?> GetByIdAsync(Guid id)
        {
            return await _context.Devices
                .Include(d => d.DevicePoints)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Device>> GetByControllerIdAsync(Guid controllerId)
        {
            return await _context.Devices
                .Where(x => x.ControllerId == controllerId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<List<Device>> GetAllAsync()
        {
            return await _context.Devices.ToListAsync();
        }
        public async Task UpdateAsync(Device device)
        {
            _context.Devices.Update(device);
            await Task.CompletedTask;
        }
    }
}
