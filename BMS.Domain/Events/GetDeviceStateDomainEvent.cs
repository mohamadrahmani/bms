using BMS.Domain.Entities.BMS;
using System;

namespace BMS.Domain.Events
{
    public record GetDeviceStateDomainEvent(Guid DeviceId, Device device) : IDomainEvent;
}
