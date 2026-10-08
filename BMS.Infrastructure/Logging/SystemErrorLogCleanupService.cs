using BMS.Application.Common.Settings;
using BMS.Domain.Entities.Logs;
using BMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BMS.Infrastructure.Logging;

public sealed class SystemErrorLogCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptionsMonitor<SystemErrorLogOptions> _options;
    private readonly ILogger<SystemErrorLogCleanupService> _logger;

    public SystemErrorLogCleanupService(
        IServiceScopeFactory scopeFactory,
        IOptionsMonitor<SystemErrorLogOptions> options,
        ILogger<SystemErrorLogCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = GetDelayUntilNextRun();

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (OperationCanceledException ex) when (stoppingToken.IsCancellationRequested)
            {
                    break;
            }

            if (stoppingToken.IsCancellationRequested ||
                !_options.CurrentValue.CleanupEnabled)
                continue;

            try
            {
                await CleanupAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "System error log cleanup failed");
            }
        }
    }

    private async Task CleanupAsync(CancellationToken cancellationToken)
    {
        var options = _options.CurrentValue;
        var retentionDays = Math.Max(7, options.RetentionDays);
        var batchSize = Math.Clamp(options.CleanupBatchSize, 100, 5000);
        var cutoff = DateTime.UtcNow.AddDays(-retentionDays);
        var totalDeleted = 0;

        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<BMSDbContext>();

        while (!cancellationToken.IsCancellationRequested)
        {
            var deleted = await context.Set<SystemErrorLog>()
                .Where(x => x.OccurredAtUtc < cutoff)
                .OrderBy(x => x.OccurredAtUtc)
                .Take(batchSize)
                .ExecuteDeleteAsync(cancellationToken);

            totalDeleted += deleted;

            if (deleted < batchSize)
                break;

            await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
        }

        if (totalDeleted > 0)
        {
            _logger.LogInformation(
                "Deleted {Count} expired system error logs older than {CutoffUtc}",
                totalDeleted,
                cutoff);
        }
    }

    private TimeSpan GetDelayUntilNextRun()
    {
        var hour = Math.Clamp(_options.CurrentValue.CleanupHourUtc, 0, 23);
        var now = DateTime.UtcNow;
        var next = new DateTime(
            now.Year,
            now.Month,
            now.Day,
            hour,
            0,
            0,
            DateTimeKind.Utc);

        if (next <= now)
            next = next.AddDays(1);

        return next - now;
    }
}
