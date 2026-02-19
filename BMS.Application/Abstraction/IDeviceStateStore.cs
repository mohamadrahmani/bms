using BMS.Domain.Entities;

namespace BMS.Application.Interfaces
{
    public interface IDeviceStateStore
    {
        Device Get(string deviceId);
    }
}
