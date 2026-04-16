using BMS.Domain.Entities.BMS;
using System;
using System.Data;

namespace BMS.Domain.Events
{
    public record DataPointUpdatedDomainEvent(
        Guid DeviceId,
        Guid Id,
        string Tag,
        //CommandType? CommandType,
        PointType PointType,
        object? Value,
        DateTime TimestampUtc
    ) : IDomainEvent;
}
