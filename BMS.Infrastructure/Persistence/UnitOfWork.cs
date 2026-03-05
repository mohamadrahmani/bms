using BMS.Application.Common.Interfaces;
using BMS.Infrastructure.Persistence;

namespace BMS.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly BMSDbContext _context;

    public UnitOfWork(BMSDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
