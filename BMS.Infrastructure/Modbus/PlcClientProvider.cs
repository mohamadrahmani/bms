using BMS.Application.Abstraction;
using BMS.Application.Abstractions;
using BMS.Application.Common.Interfaces;
using BMS.Application.Mapping;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace BMS.Infrastructure.Modbus
{

    public class PlcClientProvider : IPlcClientProvider, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PlcClientProvider> _logger;
        private readonly SemaphoreSlim _lock = new(1, 1);

        private IReadOnlyList<IPlcClient>? _clients;
        private List<IModbusConnectionManager>? _connectionManagers;

        public PlcClientProvider(
            IServiceScopeFactory scopeFactory,
            ILogger<PlcClientProvider> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task<IEnumerable<IPlcClient>> GetClientsAsync(CancellationToken ct = default)
        {
            // سریع‌ترین مسیر: اگر کش پر است، بدون قفل برگرد
            if (_clients is not null)
                return _clients;

            await _lock.WaitAsync(ct);
            try
            {
                // double-check بعد از ورود به قفل
                if (_clients is not null)
                    return _clients;

                _logger.LogInformation("Building PLC clients from database...");
                await BuildClientsAsync(ct);
                _logger.LogInformation("PLC clients built successfully. Count = {Count}", _clients!.Count);
                return _clients;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task InvalidateAsync(CancellationToken ct = default)
        {
            await _lock.WaitAsync(ct);
            try
            {
                if (_connectionManagers is not null)
                {
                    foreach (var manager in _connectionManagers)
                    {
                        if (manager is IDisposable disposable)
                            disposable.Dispose();
                    }
                    _connectionManagers = null;
                    _logger.LogInformation("Disposed old connection managers.");
                }

                _clients = null;
                _logger.LogInformation("PLC client cache invalidated.");
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task BuildClientsAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IControllerRepository>();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();

            // خواندن از دیتابیس (همان کوئری قبلی)
            var controllers = await repo.GetActiveWithDevicesAndPointsAsync(ct);

            // تبدیل به Config
            var configs = PlcConfigMapper.ToConfigs(controllers);

            var newClients = new List<IPlcClient>();
            var newManagers = new List<IModbusConnectionManager>();

            foreach (var config in configs)
            {
                var connLogger = loggerFactory.CreateLogger<ModbusConnectionManager>();
                var connectionManager = new ModbusConnectionManager(
                    config.IpAddress,
                    config.Port,
                    connLogger);
                newManagers.Add(connectionManager);

                var deviceClients = config.Devices
                    .Select(deviceConfig =>
                        (IDeviceClient)new ModbusClient(
                            config.Name,
                            connectionManager,
                            deviceConfig))
                    .ToList();

                var client = new ModbusPlcClient(
                    config.Name,
                    connectionManager,
                    deviceClients);
                newClients.Add(client);
            }

            _clients = newClients;
            _connectionManagers = newManagers;

            if (_clients.Count == 0)
                _logger.LogWarning("No active PLC configurations found in DB.");
        }

        public void Dispose()
        {
            _lock.Dispose();
            if (_connectionManagers is not null)
            {
                foreach (var manager in _connectionManagers)
                {
                    if (manager is IDisposable disposable)
                        disposable.Dispose();
                }
            }
        }

    }
}
