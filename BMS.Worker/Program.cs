using BMS.Application.Abstractions;
using BMS.Infrastructure.Modbus;
using BMS.Worker.Abstractions;
using BMS.Worker.Transport;
using BMS.Worker.Workers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IModbusConnectionManager>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var logger = sp.GetRequiredService<ILogger<ModbusConnectionManager>>();

            var ip = config["Modbus:Ip"];
            var port = config.GetValue<int>("Modbus:Port");

            return new ModbusConnectionManager(ip!, port, logger);
        });

        services.AddSingleton<IDeviceClient, ModbusAhuClient>();
        services.AddHostedService<ModbusPollingWorker>();
        services.AddSingleton<IBackendSender, ConsoleBackendSender>();
    })
    .Build();

await host.RunAsync();
