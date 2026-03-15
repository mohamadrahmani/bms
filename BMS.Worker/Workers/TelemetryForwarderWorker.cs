using System.Net.Http.Json;
using System.Threading.Channels;
using BMS.Application.Models;

namespace BMS.Worker.Workers;

public sealed class TelemetryForwarderWorker : BackgroundService
{
    private readonly ChannelReader<TelemetryMessage> _reader;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<TelemetryForwarderWorker> _logger;

    public TelemetryForwarderWorker(
        ChannelReader<TelemetryMessage> reader,
        IHttpClientFactory httpClientFactory,
        IConfiguration config,
        ILogger<TelemetryForwarderWorker> logger)
    {
        _reader = reader;
        _httpClientFactory = httpClientFactory;
        _config = config;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var url = _config["UiSink:Url"];
        if (string.IsNullOrWhiteSpace(url))
            throw new InvalidOperationException("UiSink:Url is not configured.");

        var http = _httpClientFactory.CreateClient("UiSink");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // منتظر پیام
                var msg = await _reader.ReadAsync(stoppingToken);

                // POST به UI/Backend
                var response = await http.PostAsJsonAsync(url, msg, stoppingToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("UI sink returned {StatusCode} for PLC {Plc}",
                        (int)response.StatusCode, msg.PlcName);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Telemetry forwarder failed.");
                await Task.Delay(500, stoppingToken);
            }
        }
    }
}