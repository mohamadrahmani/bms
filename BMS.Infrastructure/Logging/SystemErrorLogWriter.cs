using BMS.Application.Common.Interfaces;
using BMS.Domain.Entities.Logs;
using BMS.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BMS.Infrastructure.Logging;

public sealed class SystemErrorLogWriter : ISystemErrorLogWriter
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SystemErrorLogWriter> _logger;

    public SystemErrorLogWriter(
        IServiceScopeFactory scopeFactory,
        ILogger<SystemErrorLogWriter> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task WriteAsync(
        SystemErrorLog log,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<BMSDbContext>();
            await context.Set<SystemErrorLog>().AddAsync(log, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            // ثبت خطا نباید خطای اصلی را پنهان یا باعث حلقه‌ی خطا شود.
            _logger.LogError(
                exception,
                "Could not persist system error log {ErrorId}",
                log.ErrorId);
        }
    }
}
