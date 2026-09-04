using BMS.Domain.Entities.Logs;

namespace BMS.Application.Common.Interfaces;

public interface ISystemErrorLogWriter
{
    Task WriteAsync(
        SystemErrorLog log,
        CancellationToken cancellationToken = default);
}
