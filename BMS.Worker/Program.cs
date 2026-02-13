using BMS.Application.Abstraction;
using BMS.Application.Abstractions;
using BMS.Application.Models;
using BMS.Infrastructure.Modbus;
using BMS.Worker.Abstractions;
using BMS.Worker.Transport;
using BMS.Worker.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Polly;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;
        services.Configure<List<PlcConfig>>(
        configuration.GetSection("Plcs"));

        services.AddSingleton<IEnumerable<IPlcClient>>(sp =>
        {
            var configs = sp.GetRequiredService<IOptions<List<PlcConfig>>>().Value;
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

            return configs.Select(cfg =>
            {
                var connectionManager = new ModbusConnectionManager(
                    cfg.Ip,
                    cfg.Port,
                    loggerFactory.CreateLogger<ModbusConnectionManager>());

                var deviceClient = new ModbusAhuClient(connectionManager);

                return new ModbusPlcClient(
                    cfg.Name,
                    connectionManager,
                    new[] { deviceClient }
                );
            }).ToList();
        });


        //services.AddSingleton<IModbusConnectionManager>(sp =>
        //{
        //    var config = sp.GetRequiredService<IConfiguration>();
        //    var logger = sp.GetRequiredService<ILogger<ModbusConnectionManager>>();

        //    var ip = config["Modbus:Ip"];
        //    var port = config.GetValue<int>("Modbus:Port");

        //    return new ModbusConnectionManager(ip!, port, logger);
        //});

        //services.AddSingleton<IDeviceClient, ModbusAhuClient>();

        //services.AddSingleton<IPlcClient>(sp =>
        //{
        //    var connectionManager = sp.GetRequiredService<IModbusConnectionManager>();
        //    var deviceClient = sp.GetRequiredService<IDeviceClient>();

        //    return new ModbusPlcClient(
        //        "PLC-1",
        //        connectionManager,
        //        new[] { deviceClient }
        //    );
        //});

        services.AddSingleton<IBackendSender, ConsoleBackendSender>();

        services.AddHostedService<ModbusPollingWorker>();
        services.AddSingleton<IEnumerable<PlcClient>>(sp =>
        {
            var configs = sp.GetRequiredService<IOptions<List<PlcConfig>>>().Value;
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

            return configs.Select(cfg =>
                new PlcClient(
                    cfg,
                    loggerFactory.CreateLogger<ModbusConnectionManager>())
            ).ToList();
        });
    })
    .Build();

await host.RunAsync();
