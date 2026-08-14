using BMS.Application.Common.Filters;
using BMS.Application.Common.Pagination;
using BMS.Application.DeviceSchedules.Dtos;
using MediatR;

namespace BMS.Application.DeviceSchedules.Queries
{
    public sealed class GetDeviceSchedulesListQuery : PagedRequest, IRequest<PagedResult<DeviceScheduleDto>>
    {
        public List<FilterDto>? Filters { get; set; }
    }
}
