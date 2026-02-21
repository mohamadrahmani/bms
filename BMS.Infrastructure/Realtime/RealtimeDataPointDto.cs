using System;

namespace BMS.Infrastructure.Realtime
{
    public record RealtimeDataPointDto(
        string DeviceId,
        string PointId,
        object? Value,
        DateTime TimestampUtc);
}   