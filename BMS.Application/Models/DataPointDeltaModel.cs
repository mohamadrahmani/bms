using System;

namespace BMS.Application.Models
{
    public record DataPointDeltaModel(
        string DeviceId,
        string PointId,
        object? Value,
        DateTime TimestampUtc);
}
