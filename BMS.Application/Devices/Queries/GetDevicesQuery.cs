using BMS.Application.Common.Pagination;
using BMS.Application.Devices.DTOs;
using BMS.Domain.Entities.BMS;
using MediatR;
using System.Collections.Generic;

namespace BMS.Application.Devices.Queries
{
    public class GetDevicesQuery : PagedRequest, IRequest<PagedResult<DeviceDto>>
    {
        // فیلترهای مکانی
        public Guid? SiteId { get; set; }
        public Guid? BuildingId { get; set; }
        public Guid? FloorId { get; set; }
        public Guid? WardId { get; set; }
        public Guid? RoomId { get; set; }

        // فیلترهای رایج
        public Guid? ControllerId { get; set; }
        public bool? IsActive { get; set; }

        // سرچ (اینجا فقط Name؛ اگر خواستی Code هم اضافه می‌کنیم)
        public string? Search { get; set; }
    }
}
