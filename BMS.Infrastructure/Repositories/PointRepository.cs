using BMS.Application.Common.Interfaces;
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
    }
}
