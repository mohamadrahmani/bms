
using BMS.Domain.Entities.BMS;

namespace BMS.Application.Common.Interfaces;

public interface ICommandDefinitionRepository
{
    IQueryable<CommandDefinition> CommandDefinitions { get; }
    Task<CommandDefinition?> GetByIdAsync(Guid id);

    // Task<IEnumerable<CommandDefinition>> GetByDeviceIdAsync(Guid deviceId);

    Task AddAsync(CommandDefinition CommandDefinition);

    Task UpdateAsync(CommandDefinition CommandDefinition);

    Task DeleteAsync(Guid id);
    Task<List<CommandDefinition>> GetAllAsync(CancellationToken cancellationToken);
}
