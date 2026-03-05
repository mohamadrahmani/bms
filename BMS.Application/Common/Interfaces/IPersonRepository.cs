using BMS.Domain.Entities;

namespace BMS.Application.Common.Interfaces;

public interface IPersonRepository
{
    Task<Person?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Person person, CancellationToken cancellationToken);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
}
