using BMS.Application.Devices.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace BMS.Application.Devices.Queries
{
    public class GetDevicesByControllerQuery : IRequest<List<DeviceDto>>
    {
        public Guid ControllerId { get; set; }

        public GetDevicesByControllerQuery(Guid controllerId)
        {
            ControllerId = controllerId;
        }
    }
}
