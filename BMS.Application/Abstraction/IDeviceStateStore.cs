using BMS.Domain.Entities;
using BMS.Domain.Entities.BMS;

namespace BMS.Application.Interfaces
{
    public interface IDeviceStateStore
    {
        Device Get(Guid deviceId);
    }
}
