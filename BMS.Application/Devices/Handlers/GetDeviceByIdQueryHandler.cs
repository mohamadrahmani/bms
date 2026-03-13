using BMS.Application.Common.Interfaces;
using BMS.Application.Devices.DTOs;
using BMS.Application.Devices.Queries;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BMS.Application.Devices.Handlers
{
    public class GetDeviceByIdQueryHandler : IRequestHandler<GetDeviceByIdQuery, DeviceDto?>
    {
        private readonly IDeviceRepository _repository;

        public GetDeviceByIdQueryHandler(IDeviceRepository repository)
        {
            _repository = repository;
        }

        public async Task<DeviceDto?> Handle(GetDeviceByIdQuery request, CancellationToken cancellationToken)
        {
            var device = await _repository.GetByIdAsync(request.Id);

            if (device == null)
                return null;

            return new DeviceDto
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
            };
        }
    }
}
