using BMS.Application.Common.Interfaces;
using BMS.Application.Devices.DTOs;
using BMS.Application.Devices.Queries;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BMS.Application.Devices.Handlers
{
    public class GetDevicesByControllerQueryHandler : IRequestHandler<GetDevicesByControllerQuery, List<DeviceDto>>
    {
        private readonly IDeviceRepository _repository;

        public GetDevicesByControllerQueryHandler(IDeviceRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<DeviceDto>> Handle(GetDevicesByControllerQuery request, CancellationToken cancellationToken)
        {
            var devices = await _repository.GetByControllerIdAsync(request.ControllerId);

            return devices.Select(device => new DeviceDto
            {
                Id = device.Id,
                ControllerId = device.ControllerId,
                Code = device.Code,
                Name = device.Name,
                Type = device.Type,
                EnableAlarming = device.EnableAlarming,
                EnableTrending = device.EnableTrending,
                IsActive = device.IsActive,
                SiteId = device.Location.SiteId,
                BuildingId = device.Location.BuildingId,
                FloorId = device.Location.FloorId,
                WardId = device.Location.WardId,
                RoomId = device.Location.RoomId
            }).ToList();
        }
    }
}
