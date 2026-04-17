using BMS.Domain.Entities;
using BMS.Domain.Entities.BMS;
using BMS.Domain.Entities.Location;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BMS.Application.Common.Interfaces;

public interface IUserRepository
{
    public IQueryable<User> Users { get; }
    Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<User?> GetByUserNameAsync(
        string userName,
        CancellationToken cancellationToken);

    Task<User?> GetByIdWithRolesAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task AddAsync(
        User user,
        CancellationToken cancellationToken);

    Task<bool> ExistsByUserNameAsync(
        string userName,
        CancellationToken cancellationToken);
    Task<List<User>> GetAllWithRolesAsync(
    CancellationToken cancellationToken);

    Task<bool> ExistsByUserNameAsync(
    string username,
    CancellationToken cancellationToken,
    Guid? excludeUserId = null);

    Task<bool> ExistsByPersonIdAsync(
    Guid personId,
    CancellationToken cancellationToken);

    Task<User?> GetActiveByUserNameAsync(
    string userName,
    CancellationToken cancellationToken);

    Task DeleteAsync(User user);

}
