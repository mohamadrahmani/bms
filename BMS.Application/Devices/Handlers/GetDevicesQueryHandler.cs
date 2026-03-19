using BMS.Application.Common.Interfaces;
using BMS.Application.Devices.DTOs;
using BMS.Application.Interfaces;
using BMS.Domain.Entities.BMS;
using MediatR;

namespace BMS.Application.Devices.Queries
{
    public class GetDevicesQueryHandler
        : IRequestHandler<GetDevicesQuery, List<DeviceDto>>
    {
        private readonly IDeviceRepository _repository;

        public GetDevicesQueryHandler(IDeviceRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<DeviceDto>> Handle(
            GetDevicesQuery request,
            CancellationToken cancellationToken)
        {
            var devices = await _repository.GetAllAsync();

            return devices.Select(d => new DeviceDto
            {
                Id = d.Id,
                ControllerId = d.ControllerId,
                Code = d.Code,
                Name = d.Name,
                Type = d.Type,
                EnableAlarming = d.EnableAlarming,
                EnableTrending = d.EnableTrending,
                IsActive = d.IsActive,
                SiteId=d.Location?.SiteId,
                BuildingId = d.Location?.BuildingId,
                FloorId = d.Location?.FloorId,
                WardId = d.Location?.WardId,
                RoomId = d.Location?.RoomId
            }).ToList();
        }
    }
}
