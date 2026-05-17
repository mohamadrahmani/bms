using BMS.Application.Models;
using BMS.Domain.Entities.BMS;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Channels;

namespace BMS.Worker.Workers;

public sealed class TelemetryForwarderWorker : BackgroundService
{
    private readonly ChannelReader<TelemetryMessage> _reader;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<TelemetryForwarderWorker> _logger;

    private readonly string _logPath;

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

        var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
        Directory.CreateDirectory(logDir);
        _logPath = Path.Combine(logDir, "telemetry_log.txt");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var urlTemplate = _config["UiSink:Url"];

        if (string.IsNullOrWhiteSpace(urlTemplate))
            throw new InvalidOperationException("UiSink:Url is not configured.");

        var http = _httpClientFactory.CreateClient("UiSink");

        while (!stoppingToken.IsCancellationRequested)
        {
            TelemetryMessage msg;

            try
            {
                msg = await _reader.ReadAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            // ذخیره پیام در فایل برای مشاهده سریع
            try
            {
                var json = JsonSerializer.Serialize(msg);

                var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {json}";

                await File.AppendAllTextAsync(
                    _logPath,
                    line + Environment.NewLine,
                    stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write telemetry log.");
            }

            var delivered = false;

            while (!delivered && !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var url = urlTemplate.Replace("{DeviceId}", msg.DeviceId.ToString());

                    var response = await http.PostAsJsonAsync(url, msg, stoppingToken);

                    if (response.IsSuccessStatusCode)
                    {
                        delivered = true;
                    }
                    else
                    {
                        _logger.LogWarning(
                            "UI sink returned {StatusCode} for PLC {Plc}",
                            (int)response.StatusCode,
                            msg.PlcName);

                        await Task.Delay(2000, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Telemetry forwarder failed. Retrying...");
                    await Task.Delay(2000, stoppingToken);
                }
            }
        }
    }
}

//using BMS.Application.Models;
//using BMS.Domain.Entities.BMS;
//using System.Net.Http.Json;
//using System.Threading.Channels;

//namespace BMS.Worker.Workers;

//public sealed class TelemetryForwarderWorker : BackgroundService
//{
//    private readonly ChannelReader<TelemetryMessage> _reader;
//    private readonly IHttpClientFactory _httpClientFactory;
//    private readonly IConfiguration _config;
//    private readonly ILogger<TelemetryForwarderWorker> _logger;

//    public TelemetryForwarderWorker(
//        ChannelReader<TelemetryMessage> reader,
//        IHttpClientFactory httpClientFactory,
//        IConfiguration config,
//        ILogger<TelemetryForwarderWorker> logger)
//    {
//        _reader = reader;
//        _httpClientFactory = httpClientFactory;
//        _config = config;
//        _logger = logger;
//    }

//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        var urlTemplate = _config["UiSink:Url"];
//        string url;
//        if (string.IsNullOrWhiteSpace(urlTemplate))
//            throw new InvalidOperationException("UiSink:Url is not configured.");

//        var http = _httpClientFactory.CreateClient("UiSink");

//        //while (!stoppingToken.IsCancellationRequested)
//        //{
//        //    try
//        //    {
//        //        // منتظر پیام
//        //        var msg = await _reader.ReadAsync(stoppingToken);

//        //        // POST به UI/Backend
//        //        url = urlTemplate.Replace("{DeviceId}", msg.DeviceId.ToString());
//        //        var response = await http.PostAsJsonAsync(url, msg, stoppingToken);

//        //        if (!response.IsSuccessStatusCode)
//        //        {
//        //            _logger.LogWarning("UI sink returned {StatusCode} for PLC {Plc}",
//        //                (int)response.StatusCode, msg.PlcName);
//        //        }
//        //    }
//        //    catch (OperationCanceledException)
//        //    {
//        //        break;
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        //_logger.LogError(ex, "Telemetry forwarder failed.");
//        //        await Task.Delay(500, stoppingToken);
//        //    }
//        //}
//        while (!stoppingToken.IsCancellationRequested)
//        {
//            TelemetryMessage msg;

//            try
//            {
//                msg = await _reader.ReadAsync(stoppingToken);
//            }
//            catch (OperationCanceledException)
//            {
//                break;
//            }

//            var delivered = false;

//            while (!delivered && !stoppingToken.IsCancellationRequested)
//            {
//                try
//                {
//                    url = urlTemplate.Replace("{DeviceId}", msg.DeviceId.ToString());
//                    var response = await http.PostAsJsonAsync(url, msg, stoppingToken);

//                    if (response.IsSuccessStatusCode)
//                    {
//                        delivered = true;
//                    }
//                    else
//                    {
//                        _logger.LogWarning("UI sink returned {StatusCode} for PLC {Plc}",
//                            (int)response.StatusCode, msg.PlcName);

//                        await Task.Delay(2000, stoppingToken); // retry delay
//                    }
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "Telemetry forwarder failed. Retrying...");
//                    await Task.Delay(2000, stoppingToken);
//                }
//            }
//        }

//    }
//}