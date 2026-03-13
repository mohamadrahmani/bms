
using BMS.Domain.Entities.BMS;

namespace BMS.Application.Common.Interfaces;

public interface IPointRepository
{
    Task<Point?> GetByIdAsync(Guid id);

    Task<IEnumerable<Point>> GetByDeviceIdAsync(Guid deviceId);

    Task AddAsync(Point point);

    Task UpdateAsync(Point point);

    Task DeleteAsync(Guid id);
    Task<List<Point>> GetAllAsync(CancellationToken cancellationToken);
}
