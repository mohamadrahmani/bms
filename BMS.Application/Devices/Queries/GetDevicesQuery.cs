using BMS.Application.Devices.DTOs;
using MediatR;
using System.Collections.Generic;

namespace BMS.Application.Devices.Queries
{
    public class GetDevicesQuery : IRequest<List<DeviceDto>>
    {
    }
}
