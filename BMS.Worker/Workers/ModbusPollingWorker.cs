using BMS.Application.Abstraction;
using BMS.Application.Abstractions;
using BMS.Application.Enum;
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
        //private readonly IModbusConnectionManager _connectionManager;
        //private readonly IEnumerable<IDeviceClient> _deviceClients;
        private readonly IEnumerable<IPlcClient> _plcClients;


        public ModbusPollingWorker(
            ILogger<ModbusPollingWorker> logger,
             IEnumerable<IPlcClient> plcClients,
            IBackendSender sender
            //IModbusConnectionManager connectionManager
            )
        {
            _logger = logger;
            _plcClients = plcClients;
            _sender = sender;
            //_connectionManager = connectionManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PLC Polling Worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                //foreach (var plc in _plcClients)
                //{
                //    var ok = await plc.TestConnectionAsync(stoppingToken);

                //    if (ok)
                //        _logger.LogInformation("PLC {Name} ONLINE", plc.Name);
                //    else
                //        _logger.LogWarning("PLC {Name} OFFLINE", plc.Name);
                //}
                foreach (var plc in _plcClients)
                {
                    var ok = await plc.TestConnectionAsync(stoppingToken);

                    if (!ok)
                    {
                        _logger.LogWarning("PLC {Name} OFFLINE", plc.Name);
                        continue;
                    }

                    _logger.LogInformation("PLC {Name} ONLINE", plc.Name);

                    await plc.TestReadAsync(stoppingToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    _logger.LogInformation("PLC Polling Worker started.");

        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        var tasks = _plcClients.Select(async plc =>
        //        {
        //            try
        //            {
        //                if (plc.State == ConnectionState.Offline)
        //                {
        //                    _logger.LogWarning("PLC {Name} is offline. Retrying slowly...", plc.Name);

        //                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        //                }

        //                var snapshots = await plc.PollAsync(stoppingToken);

        //                foreach (var snapshot in snapshots)
        //                {
        //                    await _sender.SendAsync(snapshot, stoppingToken);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, "PLC {Name} polling failed.", plc.Name);
        //            }
        //        });

        //        await Task.WhenAll(tasks);

        //        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        //    }
        //}


    }
}

