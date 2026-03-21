using BMS.Domain.Entities.Location;

namespace BMS.Application.Common.Interfaces;

public interface IRoomRepository
{
    IQueryable<Room> Rooms { get; }
    Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Room>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<Room>> GetByWardIdAsync(Guid wardId, CancellationToken cancellationToken);
    Task AddAsync(Room room, CancellationToken cancellationToken);
    Task UpdateAsync(Room room, CancellationToken cancellationToken);
    Task DeleteAsync(Room room, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid id);
}
