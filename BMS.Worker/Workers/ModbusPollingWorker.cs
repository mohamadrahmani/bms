using BMS.Application.Abstraction;
using BMS.Application.Models;
using BMS.Worker.Abstractions;
using System.Threading.Channels;

namespace BMS.Worker.Workers
{
    public class ModbusPollingWorker : BackgroundService
    {
        private readonly ILogger<ModbusPollingWorker> _logger;
        private readonly IEnumerable<IPlcClient> _plcClients;
        private readonly IBackendSender _sender;
        private readonly IPlcStateStore _stateStore;
        private readonly IPlcCommandDispatcher _dispatcher;
        private readonly ChannelWriter<TelemetryMessage> _telemetryWriter;
        public ModbusPollingWorker(
            ILogger<ModbusPollingWorker> logger,
            IEnumerable<IPlcClient> plcClients,
            IBackendSender sender,
            IPlcStateStore stateStore,
            IPlcCommandDispatcher dispatcher,
            ChannelWriter<TelemetryMessage> telemetryWriter)
        {
            _logger = logger;
            _plcClients = plcClients;
            _sender = sender;
            _stateStore = stateStore;
            _dispatcher = dispatcher;
            _telemetryWriter = telemetryWriter;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PLC Polling Worker started.");

            // 🔵 تست اولیه Write (فقط یکبار در شروع)
            //await SendStartupTestCommand(stoppingToken);

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

                var statusMsg = new TelemetryMessage
                {
                    PlcName = plc.Name,
                    IsOnline = isOnline,
                    TimestampUtc = DateTime.UtcNow,
                    DeviceId = null,
                    Points = new List<TelemetryPoint>()
                };

                // status همیشه ارسال میشه تا UI وضعیت رو بفهمه
                _telemetryWriter.TryWrite(statusMsg);

                if (!isOnline)
                {
                    _logger.LogWarning("PLC {Name} OFFLINE", plc.Name);
                    return;
                }

                _logger.LogDebug("PLC {Name} ONLINE", plc.Name);
                
                var snapshots = await plc.PollAsync(token);
                foreach (var snapshot in snapshots)
                {
                    var msg = new TelemetryMessage
                    {
                        PlcName = plc.Name,
                        IsOnline = true,
                        TimestampUtc = snapshot.Timestamp,
                        DeviceId = snapshot.DeviceId,
                        Points = snapshot.Sensors.Select(s => new TelemetryPoint
                        {
                            Id = s.SensorId,      // اگر SensorId ثابت داری عالیه
                            Code = s.Name,        // اگر داری
                            Value = s.Value
                        }).ToList()
                    };

                    if (!_telemetryWriter.TryWrite(msg))
                        _logger.LogWarning("Telemetry queue is full. Dropped message for PLC {Plc}", plc.Name);
                }
                //foreach (var snapshot in snapshots)
                //{
                //    _stateStore.Update(plc.Name, snapshot);

                //    _logger.LogInformation(
                //        "Device {DeviceId} updated. Points={Count}",
                //        snapshot.DeviceId,
                //        snapshot.Sensors.Count,
                //        snapshot.Timestamp,
                //        snapshot.Sensors.First().Value
                //        );

                //    await _sender.SendAsync(snapshot, token);
                //}
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "PLC {Name} polling failed.", plc.Name);
            }
        }

        private async Task SendStartupTestCommand(CancellationToken token)
        {
            try
            {
                await _dispatcher.SendAsync(new WritePointCommand
                {
                    PlcName = "PLC-1",
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
