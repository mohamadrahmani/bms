using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using BMS.Application.Interfaces;

namespace BMS.Infrastructure.Commanding
{
    public class CommandProcessorBackgroundService : BackgroundService
    {
        private readonly GlobalDeviceCommandQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly ConcurrentDictionary<string, SemaphoreSlim>
            _deviceLocks = new();

        private const int WorkerCount = 4;

        public CommandProcessorBackgroundService(
            GlobalDeviceCommandQueue queue,
            IServiceScopeFactory scopeFactory)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
        }

        protected override Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            var tasks = new List<Task>();

            for (int i = 0; i < WorkerCount; i++)
            {
                tasks.Add(Task.Run(() =>
                    ProcessLoop(stoppingToken), stoppingToken));
            }

            return Task.WhenAll(tasks);
        }

        private async Task ProcessLoop(
            CancellationToken ct)
        {
            await foreach (var command in _queue.Reader.ReadAllAsync(ct))
            {
                var deviceLock = _deviceLocks.GetOrAdd(
                    command.DeviceId.ToString(),
                    _ => new SemaphoreSlim(1, 1));

                await deviceLock.WaitAsync(ct);

                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var gateway = scope.ServiceProvider
                        .GetRequiredService<IDeviceCommandGateway>();

                    await gateway.ExecuteAsync(
                        command.DeviceId.ToString(),
                        "command.CommandName",
                        "command.Payload");
                }
                finally
                {
                    deviceLock.Release();
                }
            }
        }
    }
}
