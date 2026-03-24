using BMS.Domain.Entities.BMS;
using BMS.Domain.Entities.Location;

public interface IBuildingRepository
{
    IQueryable<Building> Buildings { get; }
    Task<Building?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<List<Building>> GetBySiteIdAsync(Guid siteId, CancellationToken cancellationToken);

    Task<bool> ExistsCodeAsync(Guid siteId, string code, CancellationToken cancellationToken);

    Task AddAsync(Building building, CancellationToken cancellationToken);

    void Delete(Building building);
    Task<List<Building>> GetAllAsync(CancellationToken cancellationToken);
    Task<bool> ExistsCodeAsync(string code, Guid excludeId, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

}
