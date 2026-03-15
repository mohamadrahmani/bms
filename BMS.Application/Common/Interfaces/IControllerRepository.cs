using BMS.Domain.Entities.BMS;
using Microsoft.EntityFrameworkCore;

namespace BMS.Application.Common.Interfaces;

public interface IControllerRepository
{
    IQueryable<Controller> Controllers { get; }
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task AddAsync(Controller controller, CancellationToken cancellationToken = default);
    // طبق نیاز پروژه، متدهای دیگر را اضافه کن
    Task<Controller?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAndIgnoringIdAsync(string code, CancellationToken cancellationToken, Guid currentId);
    Task<List<Controller>> GetAllAsync(CancellationToken cancellationToken);
    void Update(Controller controller);
    void Remove(Controller controller);
}
