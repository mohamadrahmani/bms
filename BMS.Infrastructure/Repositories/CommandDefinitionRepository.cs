using BMS.Application.Common.Interfaces;
using BMS.Domain.Entities.BMS;
using BMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BMS.Infrastructure.Repositories
{

    public class CommandDefinitionRepository : ICommandDefinitionRepository
    {
        private readonly BMSDbContext _context;
        public IQueryable<CommandDefinition> CommandDefinitions => _context.CommandDefinitions;

        public CommandDefinitionRepository(BMSDbContext context)
        {
            _context = context;
        }

        public async Task<CommandDefinition?> GetByIdAsync(Guid id)
        {
            return await _context.CommandDefinitions
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        //public async Task<IEnumerable<CommandDefinition>> GetByDeviceIdAsync(Guid deviceId)
        //{
        //    return await _context.CommandDefinitions
        //        .Where(x => x.DeviceId == deviceId)
        //        .ToListAsync();
        //}

        public async Task AddAsync(CommandDefinition CommandDefinition)
        {
            await _context.CommandDefinitions.AddAsync(CommandDefinition);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CommandDefinition CommandDefinition)
        {
            _context.CommandDefinitions.Update(CommandDefinition);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var CommandDefinition = await _context.CommandDefinitions.FindAsync(id);

            if (CommandDefinition == null)
                return;

            _context.CommandDefinitions.Remove(CommandDefinition);

            await _context.SaveChangesAsync();
        }
        public async Task<List<CommandDefinition>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.CommandDefinitions
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
