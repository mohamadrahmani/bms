using BMS.Application.Common.Pagination;
using BMS.Application.Devices.DTOs;
using MediatR;
using System.Collections.Generic;

namespace BMS.Application.Devices.Queries
{
    public class GetDevicesQuery : PagedRequest, IRequest<PagedResult<DeviceDto>>
    {
    }
}
