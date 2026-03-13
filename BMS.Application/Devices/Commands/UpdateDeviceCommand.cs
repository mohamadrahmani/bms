using BMS.Application.Devices.DTOs;
using MediatR;

namespace BMS.Application.Devices.Commands
{
    public class UpdateDeviceCommand : IRequest<DeviceDto>
    {
        public UpdateDeviceDto Device { get; set; }

        public UpdateDeviceCommand(UpdateDeviceDto device)
        {
            Device = device;
        }
    }
}
