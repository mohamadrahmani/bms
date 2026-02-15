using BMS.Application.Abstraction;
using BMS.Application.Abstractions;
using BMS.Application.Models;
using BMS.Infrastructure.Modbus;
using BMS.Worker.Abstractions;
using BMS.Worker.Devices;
using BMS.Worker.Transport;
using BMS.Worker.Workers;
using Microsoft.Extensions.Options;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // ---------------------------
        // Bind PLC configs
        // ---------------------------
        services.Configure<List<PlcConfig>>(
            configuration.GetSection("Plcs"));

        services.AddSingleton<IPlcStateStore, InMemoryPlcStateStore>();
        services.AddSingleton<IBackendSender, ConsoleBackendSender>();
        services.AddSingleton<IPlcCommandDispatcher, PlcCommandDispatcher>();

        // ---------------------------
        // Multi-PLC Registration
        // ---------------------------
        services.AddSingleton<IPlcClient>(sp =>
        {
            var plcConfigs = sp.GetRequiredService<IOptions<List<PlcConfig>>>().Value;
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

            if (plcConfigs == null || plcConfigs.Count == 0)
                throw new InvalidOperationException("No PLC configuration found.");

            var clients = new List<IPlcClient>();

            foreach (var plcConfig in plcConfigs)
            {
                var connectionManager = new ModbusConnectionManager(
                    plcConfig.Ip,
                    plcConfig.Port,
                    loggerFactory.CreateLogger<ModbusConnectionManager>()
                );

                var deviceClients = plcConfig.Devices
                    .Select(deviceConfig =>
                        (IDeviceClient)new ModbusAhuClient(
                            connectionManager,
                            deviceConfig))
                    .ToList();

                var plcClient = new ModbusPlcClient(
                    plcConfig.Name,
                    connectionManager,
                    deviceClients
                );

                clients.Add(plcClient);
            }

            // مهم: اینجا فقط اولین PLC را برمی‌گردانیم؟
            // ❌ نه
            // چون AddSingleton<IPlcClient> فقط یکی می‌سازد

            throw new InvalidOperationException(
                "Use AddSingleton<IEnumerable<IPlcClient>>() instead.");
        });

        // ✅ روش درست برای Multi-PLC
        services.AddSingleton<IEnumerable<IPlcClient>>(sp =>
        {
            var plcConfigs = sp.GetRequiredService<IOptions<List<PlcConfig>>>().Value;
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

            if (plcConfigs == null || plcConfigs.Count == 0)
                throw new InvalidOperationException("No PLC configuration found.");

            return plcConfigs.Select(plcConfig =>
            {
                var connectionManager = new ModbusConnectionManager(
                    plcConfig.Ip,
                    plcConfig.Port,
                    loggerFactory.CreateLogger<ModbusConnectionManager>()
                );

                var deviceClients = plcConfig.Devices
                    .Select(deviceConfig =>
                        (IDeviceClient)new ModbusAhuClient(
                            connectionManager,
                            deviceConfig))
                    .ToList();

                return (IPlcClient)new ModbusPlcClient(
                    plcConfig.Name,
                    connectionManager,
                    deviceClients
                );
            }).ToList();
        });

        services.AddHostedService<ModbusPollingWorker>();
    })
    .Build();

await host.RunAsync();
