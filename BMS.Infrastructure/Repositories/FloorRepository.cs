using BMS.Domain.Entities.Location;
using BMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class FloorRepository : IFloorRepository
{
    private readonly BMSDbContext _context;

    public FloorRepository(BMSDbContext context)
    {
        _context = context;
    }

    public async Task<Floor?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Floors
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<Floor>> GetByBuildingIdAsync(Guid buildingId, CancellationToken cancellationToken)
    {
        return await _context.Floors
            .Where(x => x.BuildingId == buildingId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Floor>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Floors
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Floor floor, CancellationToken cancellationToken)
    {
        await _context.Floors.AddAsync(floor, cancellationToken);
    }

    public void Update(Floor floor)
    {
        _context.Floors.Update(floor);
    }

    public void Remove(Floor floor)
    {
        _context.Floors.Remove(floor);
    }
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Floors.AnyAsync(x => x.Id == id);
    }

}
