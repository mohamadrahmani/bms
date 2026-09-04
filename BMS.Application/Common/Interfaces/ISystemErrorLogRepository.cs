using BMS.Domain.Entities.Logs;

namespace BMS.Application.Common.Interfaces;

public interface ISystemErrorLogRepository
{
    IQueryable<SystemErrorLog> Logs { get; }

    Task<SystemErrorLog?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        SystemErrorLog log,
        CancellationToken cancellationToken = default);

    Task<bool> ResolveAsync(
        Guid id,
        Guid? resolvedByUserId,
        string? resolutionNote,
        CancellationToken cancellationToken = default);
}
