using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Pagination;
using BMS.Application.Devices.DTOs;
using BMS.Application.Interfaces;
using BMS.Domain.Entities.BMS;
using MediatR;

namespace BMS.Application.Devices.Queries
{
    public class GetDevicesQueryHandler
        : IRequestHandler<GetDevicesQuery, PagedResult<DeviceDto>>
    {
        private readonly IDeviceRepository _repository;

        public GetDevicesQueryHandler(IDeviceRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<DeviceDto>> Handle(
            GetDevicesQuery request,
            CancellationToken cancellationToken)
        {
            var devices = _repository.Devices;
            // ---- فیلترهای مکانی ----
            if (request.SiteId.HasValue)
                devices = devices.Where(d => d.Location != null && d.Location.SiteId == request.SiteId);

            if (request.BuildingId.HasValue)
                devices = devices.Where(d => d.Location != null && d.Location.BuildingId == request.BuildingId);

            if (request.FloorId.HasValue)
                devices = devices.Where(d => d.Location != null && d.Location.FloorId == request.FloorId);

            if (request.WardId.HasValue)
                devices = devices.Where(d => d.Location != null && d.Location.WardId == request.WardId);

            if (request.RoomId.HasValue)
                devices = devices.Where(d => d.Location != null && d.Location.RoomId == request.RoomId);

            // ---- سرچ فقط روی Name ----
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                devices = devices.Where(d => d.Name.Contains(request.Search));
                // اگر خواستی Code هم سرچ شود:
                // devices = devices.Where(d => d.Name.Contains(request.Search) || d.Code.Contains(request.Search));
            }
            var query = devices.Select(d => new DeviceDto
            {
                Id = d.Id,
                ControllerId = d.ControllerId,
                Code = d.Code,
                Name = d.Name,
                Type = d.Type,
                TypeName = d.Type.ToString(),
                EnableAlarming = d.EnableAlarming,
                EnableTrending = d.EnableTrending,
                IsActive = d.IsActive,
                Description = d.Description,
                SiteId = d.Location != null ? d.Location!.SiteId : null,
                BuildingId = d.Location != null ? d.Location!.BuildingId : null,
                FloorId = d.Location != null ? d.Location!.FloorId : null,
                WardId = d.Location != null ? d.Location!.WardId : null,
                RoomId = d.Location != null ? d.Location!.RoomId : null
            });

            return await query.ToPagedResultAsync(
                request.PageNumber,
                request.PageSize,
                cancellationToken);
        }
    }
}
