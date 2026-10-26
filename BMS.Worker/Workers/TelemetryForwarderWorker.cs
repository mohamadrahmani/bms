using BMS.Application.Models;
using BMS.Domain.Entities.BMS;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Channels;
using System.Diagnostics;
using BMS.Worker.Diagnostics;

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

            var queueAgeMs = TelemetryQueueDiagnostics.TrackDequeuedAndGetAgeMs(msg);
            _logger.LogDebug(
                "Telemetry dequeued. Type={MessageType}, PLC={PlcName}, DeviceId={DeviceId}, QueueAgeMs={QueueAgeMs}, QueuePending={QueuePending}",
                msg.Type, msg.PlcName, msg.DeviceId, queueAgeMs, TelemetryQueueDiagnostics.PendingTracked);

            // ذخیره پیام در فایل برای مشاهده سریع
            //try
            //{
            //    var json = JsonSerializer.Serialize(msg);

            //    var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {json}";

            //    await File.AppendAllTextAsync(
            //        _logPath,
            //        line + Environment.NewLine,
            //        stoppingToken);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Failed to write telemetry log.");
            //}

            var delivered = false;
            var attempt = 0;
            var messageTimer = Stopwatch.StartNew();

            while (!delivered && !stoppingToken.IsCancellationRequested)
            {
                attempt++;
                var sendTimer = Stopwatch.StartNew();
                try
                {
                    var response = await http.PostAsJsonAsync(urlTemplate, msg, stoppingToken);
                    sendTimer.Stop();

                    if (response.IsSuccessStatusCode)
                    {
                        delivered = true;
                        _logger.LogDebug(
                            "Telemetry delivered. Type={MessageType}, PLC={PlcName}, DeviceId={DeviceId}, Attempt={Attempt}, HttpStatus={StatusCode}, SendDurationMs={SendDurationMs}",
                            msg.Type, msg.PlcName, msg.DeviceId, attempt, (int)response.StatusCode, sendTimer.ElapsedMilliseconds);
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Telemetry delivery failed. Type={MessageType}, PLC={PlcName}, DeviceId={DeviceId}, Attempt={Attempt}, HttpStatus={StatusCode}, SendDurationMs={SendDurationMs}",
                            msg.Type, msg.PlcName, msg.DeviceId, attempt, (int)response.StatusCode, sendTimer.ElapsedMilliseconds);

                        await Task.Delay(2000, stoppingToken);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    sendTimer.Stop();
                    _logger.LogError(ex,
                        "Telemetry send exception. Type={MessageType}, PLC={PlcName}, DeviceId={DeviceId}, Attempt={Attempt}, SendDurationMs={SendDurationMs}. Retrying.",
                        msg.Type, msg.PlcName, msg.DeviceId, attempt, sendTimer.ElapsedMilliseconds);
                    await Task.Delay(2000, stoppingToken);
                }
            }

            messageTimer.Stop();
            if (delivered)
            {
                _logger.LogInformation(
                    "Telemetry message processing completed. Type={MessageType}, PLC={PlcName}, DeviceId={DeviceId}, QueueAgeMs={QueueAgeMs}, Attempts={Attempts}, TotalDeliveryMs={TotalDeliveryMs}",
                    msg.Type, msg.PlcName, msg.DeviceId, queueAgeMs, attempt, messageTimer.ElapsedMilliseconds);
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