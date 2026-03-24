using BMS.Domain.Entities.BMS;
using BMS.Domain.Entities.Location;

public interface IFloorRepository
{
    IQueryable<Floor> Floors { get; }
    Task<Floor?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<List<Floor>> GetByBuildingIdAsync(Guid buildingId, CancellationToken cancellationToken);

    Task<List<Floor>> GetAllAsync(CancellationToken cancellationToken);

    Task AddAsync(Floor floor, CancellationToken cancellationToken);

    void Update(Floor floor);

    void Remove(Floor floor);
    Task<bool> ExistsAsync(Guid id);

}
