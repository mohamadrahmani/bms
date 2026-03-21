using BMS.Application.Common.Interfaces;
using BMS.Domain.Entities.Location;
using BMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class WardRepository : IWardRepository
{
    private readonly BMSDbContext _context;

    public WardRepository(BMSDbContext context)
    {
        _context = context;
    }

    public async Task<Ward?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Wards
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<Ward>> GetByFloorIdAsync(Guid floorId, CancellationToken cancellationToken = default)
    {
        return await _context.Wards
            .Where(x => x.FloorId == floorId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Ward>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Wards
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Ward ward, CancellationToken cancellationToken = default)
    {
        await _context.Wards.AddAsync(ward, cancellationToken);
    }

    public void Update(Ward ward)
    {
        _context.Wards.Update(ward);
    }

    public void Remove(Ward ward)
    {
        _context.Wards.Remove(ward);
    }
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Wards.AnyAsync(w => w.Id == id);
    }

}
