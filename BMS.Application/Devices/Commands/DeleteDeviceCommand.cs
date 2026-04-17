using BMS.Application.Attributes;
using BMS.Domain.Entities.Logs;
using MediatR;
using System;

namespace BMS.Application.Devices.Commands
{
    [Audit(EventType.DeleteData, "Devices")]
    public class DeleteDeviceCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public DeleteDeviceCommand(Guid id)
        {
            Id = id;
        }
    }
}
