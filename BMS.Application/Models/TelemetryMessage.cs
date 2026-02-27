namespace BMS.Application.Models;

public sealed class TelemetryMessage
{
    /// <summary>
    /// Message kind.
    /// telemetry: normal device points
    /// plcStatus: PLC online/offline heartbeat
    /// commandResult: write command acknowledgement/result
    /// </summary>
    public string Type { get; set; } = "telemetry";

    public string PlcName { get; set; } = default!;
    public bool IsOnline { get; set; }
    public DateTime TimestampUtc { get; set; }

    // اگر پیام status-only بود DeviceId می‌تونه null باشه
    public Guid? DeviceId { get; set; }

    // برای UI و دموی بهتر
    public string? DeviceName { get; set; }

    // فقط برای commandResult (اختیاری)
    public bool? Success { get; set; }
    public string? Error { get; set; }

    public List<TelemetryPoint> Points { get; set; } = new();
}

public sealed class TelemetryPoint
{
    // اگر SensorId ثابت داری، از Id استفاده کن
    public Guid Id { get; set; }

    // اگر UI با کد کار می‌کنه (بهتر)، این رو هم بفرست
    public string? Code { get; set; }

    public double Value { get; set; }
}