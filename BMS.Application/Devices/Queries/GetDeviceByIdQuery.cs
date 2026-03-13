using BMS.Application.Devices.DTOs;
using MediatR;
using System;

namespace BMS.Application.Devices.Queries
{
    public class GetDeviceByIdQuery : IRequest<DeviceDto?>
    {
        public Guid Id { get; set; }

        public GetDeviceByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
