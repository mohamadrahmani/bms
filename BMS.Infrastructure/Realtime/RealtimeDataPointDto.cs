using System;

namespace BMS.Infrastructure.Realtime
{
    public record RealtimeDataPointDto(
        Guid DeviceId,
        Guid PointId,
        object? Value,
        DateTime TimestampUtc);
}   