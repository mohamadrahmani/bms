using BMS.Application.Abstraction;
using BMS.Application.Common.Interfaces;
using BMS.Application.Mapping;
using BMS.Application.Models;
using BMS.Infrastructure.Modbus;
using BMS.Worker.Abstractions;
using System.Threading.Channels;
using System.Diagnostics;
using BMS.Worker.Diagnostics;

namespace BMS.Worker.Workers
{
    public class ModbusPollingWorker : BackgroundService
    {
        private readonly ILogger<ModbusPollingWorker> _logger;
        //private readonly IEnumerable<IPlcClient> _plcClients;
        private readonly IPlcClientProvider _plcProvider;   // جایگزین _plcClients
        private readonly IBackendSender _sender;
        private readonly IPlcStateStore _stateStore;
        private readonly IPlcCommandDispatcher _dispatcher;
        private readonly ChannelWriter<TelemetryMessage> _telemetryWriter;
        private readonly IServiceScopeFactory _scopeFactory;
        public ModbusPollingWorker(
            ILogger<ModbusPollingWorker> logger,
            //IEnumerable<IPlcClient> plcClients,
            IPlcClientProvider plcProvider,
            IBackendSender sender,
            IPlcStateStore stateStore,
            IPlcCommandDispatcher dispatcher,
            ChannelWriter<TelemetryMessage> telemetryWriter,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            //_plcClients = plcClients;
            _plcProvider = plcProvider;
            _sender = sender;
            _stateStore = stateStore;
            _dispatcher = dispatcher;
            _telemetryWriter = telemetryWriter;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PLC Polling Worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var cycleId = Guid.NewGuid().ToString("N")[..8];
                var cycleTimer = Stopwatch.StartNew();
                var cycleStartedUtc = DateTimeOffset.UtcNow;

                try
                {
                    var providerTimer = Stopwatch.StartNew();
                    var plcClients = await _plcProvider.GetClientsAsync(stoppingToken);
                    providerTimer.Stop();

                    _logger.LogInformation(
                        "PollingCycle {CycleId} started at {StartedUtc}. PlcCount={PlcCount}, ProviderMs={ProviderMs}, QueuePending={QueuePending}, DroppedOldestTotal={DroppedOldestTotal}",
                        cycleId, cycleStartedUtc, plcClients.Count, providerTimer.ElapsedMilliseconds,
                        TelemetryQueueDiagnostics.PendingTracked, TelemetryQueueDiagnostics.DroppedOldest);

                    var pollingTasks = plcClients.Select(plc =>
                        PollPlcAsync(plc, cycleId, stoppingToken));

                    await Task.WhenAll(pollingTasks);

                    cycleTimer.Stop();
                    _logger.LogInformation(
                        "PollingCycle {CycleId} completed. DurationMs={DurationMs}, QueuePending={QueuePending}, DroppedOldestTotal={DroppedOldestTotal}",
                        cycleId, cycleTimer.ElapsedMilliseconds,
                        TelemetryQueueDiagnostics.PendingTracked, TelemetryQueueDiagnostics.DroppedOldest);

                    await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
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
        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    _logger.LogInformation("PLC Polling Worker started.");

        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        try
        //        {
        //            var pollingTasks = _plcClients.Select(plc =>
        //                PollPlcAsync(plc, stoppingToken));

        //            await Task.WhenAll(pollingTasks);

        //            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        //        }
        //        catch (OperationCanceledException)
        //        {
        //            break;
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "Unexpected worker error.");
        //            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        //        }
        //    }

        //    _logger.LogInformation("PLC Polling Worker stopped.");
        //}

        private async Task PollPlcAsync(IPlcClient plc, string cycleId, CancellationToken token)
        {
            var plcTimer = Stopwatch.StartNew();
            try
            {
                var swTest = Stopwatch.StartNew();
                var isOnline = await plc.TestConnectionAsync(token);
                swTest.Stop();
                _logger.LogInformation(
                    "PollingCycle {CycleId} PLC {PlcName} connection test completed. DurationMs={DurationMs}, Online={Online}",
                    cycleId, plc.Name, swTest.ElapsedMilliseconds, isOnline);
                //var statusMsg = new TelemetryMessage
                //{
                //    Type = "plcStatus",
                //    PlcName = plc.Name,
                //    IsOnline = isOnline,
                //    TimestampUtc = DateTime.UtcNow,
                //    DeviceId = null,
                //    Points = new List<TelemetryPoint>()
                //};

                // status همیشه ارسال میشه تا UI وضعیت رو بفهمه
                //_telemetryWriter.TryWrite(statusMsg); //فعلا کنسلش میکنم تا در صورت نیاز بعدا فعال بشه(فقط وضعیت کنترلر رو رارسال میکنه)

                if (!isOnline)
                {
                    //_logger.LogWarning("PLC {Name} OFFLINE", plc.Name);
                    var offlineMessage = new TelemetryMessage
                    {
                        Type = "telemetry",
                        PlcName = plc.Name,
                        IsOnline = false,
                        TimestampUtc = DateTime.UtcNow,
                        Points = new()
                    };
                    TelemetryQueueDiagnostics.TrackEnqueued(offlineMessage);
                    if (!_telemetryWriter.TryWrite(offlineMessage))
                    {
                        TelemetryQueueDiagnostics.TrackWriteFailed(offlineMessage);
                        _logger.LogWarning("Telemetry queue write failed for offline PLC {PlcName}. CycleId={CycleId}", plc.Name, cycleId);
                    }

                    return;
                }


                _logger.LogInformation("PLC {Name} ONLINE", plc.Name);

                var pollTimer = Stopwatch.StartNew();
                var snapshots = await plc.PollAsync(token);
                pollTimer.Stop();
                _logger.LogInformation(
                    "PollingCycle {CycleId} PLC {PlcName} poll completed. DurationMs={DurationMs}, SnapshotCount={SnapshotCount}",
                    cycleId, plc.Name, pollTimer.ElapsedMilliseconds, snapshots.Count);

                foreach (var snapshot in snapshots)
                {
                    var msg = new TelemetryMessage
                    {
                        Type = "telemetry",
                        PlcName = plc.Name,
                        IsOnline = true,
                        TimestampUtc = snapshot.Timestamp,
                        DeviceId = snapshot.DeviceId,
                        DeviceName = snapshot.DeviceName,
                        Points = snapshot.Sensors.Select(s => new TelemetryPoint
                        {
                            Id = s.SensorId,      // اگر SensorId ثابت داری عالیه
                            Code = s.Name,        // اگر داری
                            Value = s.Value
                        }).ToList()

                    };
                    //await _sender.SendAsync(snapshot, token);
                    TelemetryQueueDiagnostics.TrackEnqueued(msg);
                    if (!_telemetryWriter.TryWrite(msg))
                    {
                        TelemetryQueueDiagnostics.TrackWriteFailed(msg);
                        _logger.LogWarning("Telemetry queue write failed for PLC {PlcName}, Device {DeviceId}. CycleId={CycleId}", plc.Name, snapshot.DeviceId, cycleId);
                    }
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


                //}
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                _logger.LogDebug("PollingCycle {CycleId} PLC {PlcName} cancelled.", cycleId, plc.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PollingCycle {CycleId} PLC {PlcName} polling failed.", cycleId, plc.Name);
            }
            finally
            {
                plcTimer.Stop();
                _logger.LogInformation(
                    "PollingCycle {CycleId} PLC {PlcName} finished. TotalDurationMs={DurationMs}",
                    cycleId, plc.Name, plcTimer.ElapsedMilliseconds);
            }
        }

        // Startup test command removed for demo stability.
    }
}
