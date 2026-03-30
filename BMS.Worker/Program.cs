using BMS.Application.Abstraction;
using BMS.Application.Abstractions;
using BMS.Application.Common.Interfaces;
using BMS.Application.Interfaces;
using BMS.Application.Mapping;
using BMS.Application.Models;
using BMS.Application.Points.Dtos;
using BMS.Application.Utilities;
using BMS.Domain.Events;
using BMS.Infrastructure;
using BMS.Infrastructure.Modbus;
using BMS.Infrastructure.Persistence;
using BMS.Worker.Abstractions;
using BMS.Worker.Devices;
using BMS.Worker.Transport;
using BMS.Worker.Workers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// ---------------------------
// Config
// ---------------------------
//builder.Services.Configure<List<PlcConfig>>(configuration.GetSection("Plcs"));
builder.Services.RemoveAll<IEventHandler<DeviceCommandCompletedDomainEvent>>();
builder.Services.AddInfrastructure(builder.Configuration, enableRealtime: false);
// ---------------------------
// Demo-friendly CORS (UI calls Worker directly)
// ---------------------------
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ---------------------------
// Core services
// ---------------------------
builder.Services.AddSingleton<IPlcStateStore, InMemoryPlcStateStore>();
builder.Services.AddSingleton<IBackendSender, ConsoleBackendSender>();
builder.Services.AddSingleton<IPlcCommandDispatcher, PlcCommandDispatcher>();

// ---------------------------
// Multi-PLC registration
// ---------------------------
//builder.Services.AddSingleton<IEnumerable<IPlcClient>>(sp =>
//{
//    var plcConfigs = sp.GetRequiredService<IOptions<List<PlcConfig>>>().Value;
//    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

//    if (plcConfigs == null || plcConfigs.Count == 0)
//        throw new InvalidOperationException("No PLC configuration found.");

//    return plcConfigs.Select(plcConfig =>
//    {
//        var connectionManager = new ModbusConnectionManager(
//            plcConfig.IpAddress,
//            plcConfig.Port,
//            loggerFactory.CreateLogger<ModbusConnectionManager>()
//        );

//        var deviceClients = plcConfig.Devices
//            .Select(deviceConfig =>
//                (IDeviceClient)new ModbusAhuClient(
//                    plcConfig.Name,
//                    connectionManager,
//                    deviceConfig))
//            .ToList();

//        return (IPlcClient)new ModbusPlcClient(
//            plcConfig.Name,
//            connectionManager,
//            deviceClients
//        );
//    }).ToList();
//});

builder.Services.AddSingleton<IEnumerable<IPlcClient>>(sp =>
{
    using var scope = sp.CreateScope();
    var scoped = scope.ServiceProvider;
    var scopedProvider = scope.ServiceProvider;

    var logger = scopedProvider
        .GetRequiredService<ILogger<Program>>();

    // ۱. گرفتن Repository
    var controllerRepo = scopedProvider
        .GetRequiredService<IControllerRepository>();

    var loggerFactory = scoped.GetRequiredService<ILoggerFactory>();
    // ۲. خواندن از DB (sync در زمان بوت)
    var controllers = controllerRepo
        .GetActiveWithDevicesAndPointsAsync(CancellationToken.None)
        .GetAwaiter().GetResult();

    // ۳. تبدیل Domain به PlcConfig (Mapperی که در Application ساخته‌ای)

    var plcConfigs = PlcConfigMapper.ToConfigs(controllers);

    // ۴. ساختن PlcClient برای هر Config
    var clients = plcConfigs.Select(config =>
    {
        var connectionLogger =
    loggerFactory.CreateLogger<ModbusConnectionManager>();
        // چیزهایی که ModbusPlcClient لازم دارد:
        //var connectionManager = scopedProvider
        //    .GetRequiredService<IModbusConnectionManager>();
        var connectionManager = new ModbusConnectionManager(
    config.IpAddress,
    config.Port,
    connectionLogger
    );

        //var deviceClients = scopedProvider
        //    .GetRequiredService<IEnumerable<IDeviceClient>>();
        var deviceClients = config.Devices
    .Select(deviceConfig =>
        (IDeviceClient)new ModbusAhuClient(
            config.Name,
            connectionManager,
            deviceConfig))
    .ToList();

        return (IPlcClient)new ModbusPlcClient(
            config.Name,
            connectionManager,
            deviceClients /* یا فیلتر شده بر اساس config */
        );
    }).ToList();

    if (!clients.Any())
    {
        logger.LogWarning("No active PLC configurations found in DB.");
    }

    return clients;
});

// ---------------------------
// Telemetry queue
// ---------------------------
builder.Services.AddSingleton(sp =>
{
    var options = new BoundedChannelOptions(capacity: 500)
    {
        SingleReader = true,
        SingleWriter = false,
        // برای دمو بهتره Polling گیر نکنه:
        FullMode = BoundedChannelFullMode.DropOldest
    };

    return Channel.CreateBounded<TelemetryMessage>(options);
});
builder.Services.AddSingleton(sp => sp.GetRequiredService<Channel<TelemetryMessage>>().Writer);
builder.Services.AddSingleton(sp => sp.GetRequiredService<Channel<TelemetryMessage>>().Reader);

builder.Services.AddHttpClient("UiSink");

//______________________________________________________

//______________________________________________________
// ---------------------------
// Hosted services
// ---------------------------
builder.Services.AddHostedService<ModbusPollingWorker>();
builder.Services.AddHostedService<TelemetryForwarderWorker>();

// ---------------------------
// Worker Command API URL
// ---------------------------
var commandUrl = configuration["CommandApi:Url"];
builder.WebHost.UseUrls(string.IsNullOrWhiteSpace(commandUrl) ? "http://localhost:5055" : commandUrl);

var app = builder.Build();

app.UseCors();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// -------------------------------------------------
// Command ingress (demo): UI -> Worker -> PLC
// -------------------------------------------------
app.MapPost("/api/commands/write-point",
    async (
        WritePointRequest request,
         IPointRepository pointRepository,
        IPlcCommandDispatcher dispatcher,
        ChannelWriter<TelemetryMessage> telemetryWriter,
        CancellationToken token) =>
    {
        var point = await pointRepository.GetPointFullInfoAsync(request.PointId);
        if (point == null)
            return Results.NotFound(new { error = "Point not found" });

        //// Basic validation
        //if (string.IsNullOrWhiteSpace(command.PlcName))
        //    return Results.BadRequest(new { error = "PlcName is required." });
        //if (string.IsNullOrWhiteSpace(command.DeviceName))
        //    return Results.BadRequest(new { error = "DeviceName is required." });
        //if (string.IsNullOrWhiteSpace(command.PointCode))
        //    return Results.BadRequest(new { error = "PointCode is required." });

        // Ensure stable routing key
        //if (command.DeviceId == Guid.Empty)
        //    command.DeviceId = DeterministicGuid.FromString($"bms|plc:{command.PlcName}|device:{command.DeviceName}");

        //var pointId = DeterministicGuid.FromString($"bms|plc:{command.PlcName}|device:{command.DeviceName}|point:{command.PointCode}");

        bool success;
        string? error = null;
        try
        {
            success = await dispatcher.SendAsync(point, request.Value, token);
        }
        catch (Exception ex)
        {
            success = false;
            error = ex.Message;
        }

        // Push a commandResult message to UI sink (best-effort)
        telemetryWriter.TryWrite(new TelemetryMessage
        {
            Type = "commandResult",
            PlcName = point.ControllerName,
            IsOnline = true,
            TimestampUtc = DateTime.UtcNow,
            DeviceId = point.DeviceId,
            DeviceName = point.DeviceName,
            Success = success,
            Error = error,
            Points = new List<TelemetryPoint>
            {
                new()
                {
                    Id = point.Id,
                    Code = point.Code,
                    Value = double.Parse(request.Value)
                }
            }
        });

        return Results.Ok(new
        {
            success,
            error,
            deviceId = point.DeviceId,
            PointId=point.Id,
            value=request.Value
        });
    });

await app.RunAsync();
