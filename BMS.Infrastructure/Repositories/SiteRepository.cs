using BMS.Application.Common.Interfaces;
using BMS.Domain.Entities.BMS;
using BMS.Domain.Entities.Location;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace BMS.Infrastructure.Persistence.Repositories;

public class SiteRepository : ISiteRepository
{
    private readonly BMSDbContext _db;

    public SiteRepository(BMSDbContext db)
    {
        _db = db;
    }
    public IQueryable<Site> Sites => _db.Sites;
    public async Task<Site?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _db.Sites
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _db.Sites
            .AnyAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Site>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Sites
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Site site, CancellationToken cancellationToken = default)
    {
        await _db.Sites.AddAsync(site, cancellationToken);
    }

    public void Delete(Site site)
    {
        _db.Sites.Remove(site);
    }
}
