using Microsoft.EntityFrameworkCore;
using BMS.Application.Common.Interfaces;
using BMS.Domain.Entities.Location;
using BMS.Infrastructure.Persistence;

public class BuildingRepository : IBuildingRepository
{
    private readonly BMSDbContext _context;

    public BuildingRepository(BMSDbContext context)
    {
        _context = context;
    }

    public async Task<Building?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Buildings
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<Building>> GetBySiteIdAsync(Guid siteId, CancellationToken cancellationToken)
    {
        return await _context.Buildings
            .Where(x => x.SiteId == siteId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsCodeAsync(Guid siteId, string code, CancellationToken cancellationToken)
    {
        return await _context.Buildings
            .AnyAsync(x => x.SiteId == siteId && x.Code == code, cancellationToken);
    }

    public async Task AddAsync(Building building, CancellationToken cancellationToken)
    {
        await _context.Buildings.AddAsync(building, cancellationToken);
    }

    public void Delete(Building building)
    {
        _context.Buildings.Remove(building);
    }
    public async Task<List<Building>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Buildings
            .ToListAsync(cancellationToken);
    }
    public async Task<bool> ExistsCodeAsync(string code, Guid excludeId, CancellationToken cancellationToken)
    {
        return await _context.Buildings
            .AnyAsync(x => x.Code == code && x.Id != excludeId, cancellationToken);
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Buildings
            .AnyAsync(b => b.Id == id, cancellationToken);
    }

}
