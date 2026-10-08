using BMS.Application.Models;
using BMS.Infrastructure.Persistence.Entities;
using BMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace BMS.Infrastructure.Historian
{
    public class HistorianBackgroundService : BackgroundService
    {
        private readonly Channel<DataPointDeltaModel> _channel;
        private readonly IServiceScopeFactory _scopeFactory;

        private const int BatchSize = 500;
        private const int FlushIntervalMs = 1000;

        public HistorianBackgroundService(
            Channel<DataPointDeltaModel> channel,
            IServiceScopeFactory scopeFactory)
        {
            _channel = channel;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            var buffer = new List<DataPointHistoryEntity>(BatchSize);
            var timer = new PeriodicTimer(
                TimeSpan.FromMilliseconds(FlushIntervalMs));

            while (!stoppingToken.IsCancellationRequested)
            {
                while (_channel.Reader.TryRead(out var delta))
                {
                    buffer.Add(Map(delta));

                    if (buffer.Count >= BatchSize)
                        await FlushAsync(buffer, stoppingToken);
                }

                await timer.WaitForNextTickAsync(stoppingToken);

                if (buffer.Count > 0)
                    await FlushAsync(buffer, stoppingToken);
            }
        }

        private async Task FlushAsync(
            List<DataPointHistoryEntity> buffer,
            CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<BMSDbContext>();

            await db.DataPointHistory.AddRangeAsync(buffer, ct);
            await db.SaveChangesAsync(ct);

            buffer.Clear();
        }

        private static DataPointHistoryEntity Map(
            DataPointDeltaModel delta)
        {
            return new DataPointHistoryEntity
            {
                DeviceId = delta.DeviceId,
                PointId = delta.PointId,
                ValueString = delta.Value?.ToString(),
                TimestampUtc = delta.TimestampUtc
            };
        }
    }
}
