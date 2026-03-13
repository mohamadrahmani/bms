using MediatR;
using System;

namespace BMS.Application.Devices.Commands
{
    public class DeleteDeviceCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public DeleteDeviceCommand(Guid id)
        {
            Id = id;
        }
    }
}
