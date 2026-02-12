using BMS.Application.Abstractions;
using BMS.Infrastructure.Modbus;
using BMS.Worker.Abstractions;
using System.Reflection;

namespace BMS.Worker.Workers
{
    public class ModbusPollingWorker : BackgroundService
    {
        private readonly ILogger<ModbusPollingWorker> _logger;
        //private readonly IDeviceClient _deviceClient;
        private readonly IBackendSender _sender;
        private readonly IModbusConnectionManager _connectionManager;
        private readonly IEnumerable<IDeviceClient> _deviceClients;


        public ModbusPollingWorker(
            ILogger<ModbusPollingWorker> logger,
            IEnumerable<IDeviceClient> deviceClients,
            IBackendSender sender,
            IModbusConnectionManager connectionManager)
        {
            _logger = logger;
            _deviceClients = deviceClients;
            _sender = sender;
            _connectionManager = connectionManager;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Modbus Polling Worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var state = _connectionManager.State;

                    switch (state)
                    {
                        case ConnectionState.Offline:
                            _logger.LogWarning(
                                "Connection is OFFLINE. ConsecutiveFailures={Failures}. Retrying in 30s...",
                                _connectionManager.ConsecutiveFailures);

                            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                            continue;

                        case ConnectionState.HalfOpen:
                            _logger.LogInformation("Connection is HALF-OPEN. Testing devices...");
                            break;

                        case ConnectionState.Online:
                        case ConnectionState.Unknown:
                        default:
                            break;
                    }

                    // 🔵 Parallel polling of all devices
                    var pollingTasks = _deviceClients.Select(async device =>
                    {
                        try
                        {
                            var snapshot = await device.ReadAsync(stoppingToken);

                            await _sender.SendAsync(snapshot, stoppingToken);

                            _logger.LogDebug(
                                "Device {DeviceId} polled successfully at {Time}",
                                snapshot.DeviceId,
                                snapshot.Timestamp);
                        }
                        catch (OperationCanceledException)
                        {
                            // graceful shutdown
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Device polling failed.");
                        }
                    });

                    await Task.WhenAll(pollingTasks);

                    // Normal polling interval when online
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Worker cancellation requested.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error in polling loop.");

                    // small delay to prevent tight crash loop
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }

            _logger.LogInformation("Modbus Polling Worker stopped.");
        }


    }
}

