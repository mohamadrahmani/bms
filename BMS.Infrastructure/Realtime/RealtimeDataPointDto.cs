using System;

namespace BMS.Infrastructure.Realtime
{
    public record RealtimeDataPointDto(
        Guid DeviceId,
        Guid Id,
        object? Value,
        DateTime TimestampUtc);
}   