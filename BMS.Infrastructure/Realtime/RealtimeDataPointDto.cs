using System;
using System.Data;
using BMS.Domain.Entities.BMS;
namespace BMS.Infrastructure.Realtime
{
    public record RealtimeDataPointDto(
        Guid DeviceId,
        Guid Id,
        string Tag,
        //CommandType CommandType,
        BMS.Domain.Entities.BMS.PointType PointType,
        object? Value,
        DateTime TimestampUtc);
}   