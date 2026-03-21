using BMS.Application.Common.Interfaces;
using BMS.Domain.Entities.BMS;
using BMS.Domain.Entities.Location;
using Microsoft.EntityFrameworkCore;

namespace BMS.Infrastructure.Persistence.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly BMSDbContext _context;
    public IQueryable<Room> Rooms => _context.Rooms;
    public RoomRepository(BMSDbContext context)
    {
        _context = context;
    }

    public async Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Rooms.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<Room>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Rooms.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<List<Room>> GetByWardIdAsync(Guid wardId, CancellationToken cancellationToken)
    {
        return await _context.Rooms
            .Where(x => x.WardId == wardId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Room room, CancellationToken cancellationToken)
    {
        await _context.Rooms.AddAsync(room, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Room room, CancellationToken cancellationToken)
    {
        _context.Rooms.Update(room);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Room room, CancellationToken cancellationToken)
    {
        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Rooms.AnyAsync(r => r.Id == id);
    }
}
