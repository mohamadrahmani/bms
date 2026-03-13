using BMS.Application.Devices.DTOs;
using MediatR;

namespace BMS.Application.Devices.Commands
{
    public class CreateDeviceCommand : IRequest<DeviceDto>
    {
        public CreateDeviceDto Device { get; set; }

        public CreateDeviceCommand(CreateDeviceDto device)
        {
            Device = device;
        }
    }
}
