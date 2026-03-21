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

            var query = devices.Select(d => new DeviceDto
            {
                Id = d.Id,
                ControllerId = d.ControllerId,
                Code = d.Code,
                Name = d.Name,
                Type = d.Type,
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

            try
            {
                var asdfa = query.ToList();
            }
            catch(Exception ex)
            {

            }
            return await query.ToPagedResultAsync(
                request.PageNumber,
                request.PageSize,
                cancellationToken);
        }
    }
}
