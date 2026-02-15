using BMS.Application.Abstraction;
using BMS.Application.Models;
using BMS.Worker.Abstractions;

namespace BMS.Worker.Workers
{
    public class ModbusPollingWorker : BackgroundService
    {
        private readonly ILogger<ModbusPollingWorker> _logger;
        private readonly IEnumerable<IPlcClient> _plcClients;
        private readonly IBackendSender _sender;
        private readonly IPlcStateStore _stateStore;
        private readonly IPlcCommandDispatcher _dispatcher;

        public ModbusPollingWorker(
            ILogger<ModbusPollingWorker> logger,
            IEnumerable<IPlcClient> plcClients,
            IBackendSender sender,
            IPlcStateStore stateStore,
            IPlcCommandDispatcher dispatcher)
        {
            _logger = logger;
            _plcClients = plcClients;
            _sender = sender;
            _stateStore = stateStore;
            _dispatcher = dispatcher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PLC Polling Worker started.");

            // 🔵 تست اولیه Write (فقط یکبار در شروع)
            await SendStartupTestCommand(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var pollingTasks = _plcClients.Select(plc =>
                        PollPlcAsync(plc, stoppingToken));

                    await Task.WhenAll(pollingTasks);

                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected worker error.");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }

            _logger.LogInformation("PLC Polling Worker stopped.");
        }

        private async Task PollPlcAsync(IPlcClient plc, CancellationToken token)
        {
            try
            {
                var isOnline = await plc.TestConnectionAsync(token);

                if (!isOnline)
                {
                    _logger.LogWarning("PLC {Name} OFFLINE", plc.Name);
                    return;
                }

                _logger.LogDebug("PLC {Name} ONLINE", plc.Name);

                var snapshots = await plc.PollAsync(token);

                foreach (var snapshot in snapshots)
                {
                    _stateStore.Update(plc.Name, snapshot);

                    _logger.LogInformation(
                        "Device {DeviceId} updated. Points={Count}",
                        snapshot.DeviceId,
                        snapshot.Sensors.Count);

                    await _sender.SendAsync(snapshot, token);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PLC {Name} polling failed.", plc.Name);
            }
        }

        private async Task SendStartupTestCommand(CancellationToken token)
        {
            try
            {
                await _dispatcher.SendAsync(new WritePointCommand
                {
                    PlcName = "PLC-1",
                    SlaveId = 1,
                    Address = 0,
                    Value = 123
                }, token);

                _logger.LogInformation("Startup test write sent.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Startup write failed.");
            }
        }
    }
}
