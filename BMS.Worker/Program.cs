using System.Threading.Channels;
using BMS.Application.Abstraction;
using BMS.Application.Abstractions;
using BMS.Application.Models;
using BMS.Application.Utilities;
using BMS.Infrastructure.Modbus;
using BMS.Worker.Abstractions;
using BMS.Worker.Devices;
using BMS.Worker.Transport;
using BMS.Worker.Workers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// ---------------------------
// Config
// ---------------------------
builder.Services.Configure<List<PlcConfig>>(configuration.GetSection("Plcs"));

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
builder.Services.AddSingleton<IEnumerable<IPlcClient>>(sp =>
{
    var plcConfigs = sp.GetRequiredService<IOptions<List<PlcConfig>>>().Value;
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

    if (plcConfigs == null || plcConfigs.Count == 0)
        throw new InvalidOperationException("No PLC configuration found.");

    return plcConfigs.Select(plcConfig =>
    {
        var connectionManager = new ModbusConnectionManager(
            plcConfig.IpAddress,
            plcConfig.Port,
            loggerFactory.CreateLogger<ModbusConnectionManager>()
        );

        var deviceClients = plcConfig.Devices
            .Select(deviceConfig =>
                (IDeviceClient)new ModbusAhuClient(
                    plcConfig.Name,
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
        WritePointCommand command,
        IPlcCommandDispatcher dispatcher,
        ChannelWriter<TelemetryMessage> telemetryWriter,
        CancellationToken token) =>
    {
        // Basic validation
        if (string.IsNullOrWhiteSpace(command.PlcName))
            return Results.BadRequest(new { error = "PlcName is required." });
        if (string.IsNullOrWhiteSpace(command.DeviceName))
            return Results.BadRequest(new { error = "DeviceName is required." });
        if (string.IsNullOrWhiteSpace(command.PointCode))
            return Results.BadRequest(new { error = "PointCode is required." });

        // Ensure stable routing key
        if (command.DeviceId == Guid.Empty)
            command.DeviceId = DeterministicGuid.FromString($"bms|plc:{command.PlcName}|device:{command.DeviceName}");

        var pointId = DeterministicGuid.FromString($"bms|plc:{command.PlcName}|device:{command.DeviceName}|point:{command.PointCode}");

        bool success;
        string? error = null;
        try
        {
            success = await dispatcher.SendAsync(command, token);
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
            PlcName = command.PlcName,
            IsOnline = true,
            TimestampUtc = DateTime.UtcNow,
            DeviceId = command.DeviceId,
            DeviceName = command.DeviceName,
            Success = success,
            Error = error,
            Points = new List<TelemetryPoint>
            {
                new()
                {
                    Id = pointId,
                    Code = command.PointCode,
                    Value = command.Value
                }
            }
        });

        return Results.Ok(new
        {
            success,
            error,
            deviceId = command.DeviceId,
            pointId
        });
    });

await app.RunAsync();
