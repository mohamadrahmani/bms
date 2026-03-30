using BMS.Application.Common.Interfaces;
using BMS.Domain.Entities;
using BMS.Domain.Entities.BMS;
using BMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace BMS.Infrastructure.Repositories
{
    public class ControllerRepository : IControllerRepository
    {
        private readonly BMSDbContext _dbContext;
        public IQueryable<Controller> Controllers => _dbContext.Controllers;

        public ControllerRepository(BMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            var normalizedCode = code.Trim().ToUpperInvariant();
            return await _dbContext.Controllers
                .AnyAsync(x => x.Code == normalizedCode, cancellationToken);
        }

        public async Task AddAsync(Controller controller, CancellationToken cancellationToken = default)
        {
            await _dbContext.Controllers.AddAsync(controller, cancellationToken);
        }

        public async Task<Controller?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Controllers
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
        public async Task<bool> ExistsByCodeAndIgnoringIdAsync(
    string code,
    CancellationToken cancellationToken,
    Guid currentId)
        {
            var normalizedCode = code.Trim().ToUpperInvariant();

            return await _dbContext.Controllers
                .AnyAsync(x =>
                    x.Code == normalizedCode &&
                    x.Id != currentId,
                    cancellationToken);
        }
        public async Task<List<Controller>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Controllers
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        public void Update(Controller controller)
        {
            _dbContext.Controllers.Update(controller);
        }
        public void Remove(Controller controller)
        {
            _dbContext.Controllers.Remove(controller);
        }

        public async Task<List<Controller>> GetActiveWithDevicesAndPointsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Controllers
                .Where(c => c.IsActive)
                .Include(c => c.Devices)
                    .ThenInclude(d => d.DevicePoints)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

    }
}
