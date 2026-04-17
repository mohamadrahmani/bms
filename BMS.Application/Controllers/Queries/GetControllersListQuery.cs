using BMS.Application.Common.Pagination;
using BMS.Application.Controllers.Dtos;
using MediatR;
using System.Collections.Generic;

namespace BMS.Application.Controllers.Queries
{
    public sealed class GetControllersListQuery : PagedRequest, IRequest<PagedResult<ControllerDto>>
    {
        // فیلترهای مکانی
        public Guid? SiteId { get; set; }
        public Guid? BuildingId { get; set; }
        public Guid? FloorId { get; set; }
        public Guid? WardId { get; set; }
        public Guid? RoomId { get; set; }

        public string? Search { get; set; }
    }
}
