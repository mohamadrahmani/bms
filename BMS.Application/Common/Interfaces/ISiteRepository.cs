using BMS.Domain.Entities.BMS;
using BMS.Domain.Entities.Location;

namespace BMS.Application.Common.Interfaces;

public interface ISiteRepository
{
    IQueryable<Site> Sites { get; }
    Task<Site?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);


    Task<IReadOnlyList<Site>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Site site, CancellationToken cancellationToken = default);

    void Delete(Site site);
}
