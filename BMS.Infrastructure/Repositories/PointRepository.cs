using BMS.Application.Common.Interfaces;
using BMS.Application.Models;
using BMS.Application.Points.Dtos;
using BMS.Domain.Entities.BMS;
using BMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BMS.Infrastructure.Repositories
{

    public class PointRepository : IPointRepository
    {
        private readonly BMSDbContext _context;
        public IQueryable<Point> Points => _context.Points;

        public PointRepository(BMSDbContext context)
        {
            _context = context;
        }

        public async Task<Point?> GetByIdAsync(Guid id)
        {
            return await _context.Points
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Point>> GetByDeviceIdAsync(Guid deviceId)
        {
            return await _context.Points
                .Where(x => x.DeviceId == deviceId)
                .ToListAsync();
        }

        public async Task AddAsync(Point point)
        {
            await _context.Points.AddAsync(point);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Point point)
        {
            _context.Points.Update(point);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var point = await _context.Points.FindAsync(id);

            if (point == null)
                return;

            _context.Points.Remove(point);

            await _context.SaveChangesAsync();
        }
        public async Task<List<Point>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Points
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<PointDto?> GetPointFullInfoAsync(Guid pointId)
        {
            return await _context.Points
                .Where(p => p.Id == pointId)
                .Select(p => new PointDto
                {
                    Id = p.Id,
                    Tag = p.Tag,
                    Title = p.Title,
                    Address = p.Address,
                    Code= p.Code,
                    //CommandAddress= p.CommandAddress,
                    FeedbackAddress= p.FeedbackAddress,
                    ValidationDelayMs= p.ValidationDelayMs,
                    ValidationRetryCount= p.ValidationRetryCount,
                    Scale=p.Scale,
                    IsWritable=p.IsWritable,
                    DataType=p.DataType,
                    RegisterType=p.RegisterType,
                    Length=p.Length,

                    DeviceId = p.Device.Id,
                    DeviceCode = p.Device.Code,
                    DeviceName = p.Device.Name,

                    ControllerId = p.Device.Controller.Id,
                    ControllerCode = p.Device.Controller.Code,
                    ControllerName = p.Device.Controller.Name,
                    IpAddress = p.Device.Controller.IpAddress
                })
                .FirstOrDefaultAsync();
        }

    }
}
