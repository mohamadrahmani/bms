using BMS.Application.Common.Interfaces;
using BMS.Domain.Entities.BMS;
using BMS.Infrastructure.Persistence;

namespace BMS.Infrastructure.Repositories;

public class PointCategoryRepository : IPointCategoryRepository
{
    private readonly BMSDbContext _context;

    public PointCategoryRepository(BMSDbContext context) => _context = context;

    public IQueryable<PointCategory> Categories => _context.PointCategories;
}
