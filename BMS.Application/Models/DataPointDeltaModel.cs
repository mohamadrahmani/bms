using System;

namespace BMS.Application.Models
{
    public record DataPointDeltaModel(
        Guid DeviceId,
        Guid PointId,
        object? Value,
        DateTime TimestampUtc);
}
