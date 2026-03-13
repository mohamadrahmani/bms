using BMS.Application.Common.Interfaces;
using BMS.Application.Devices.Commands;
using BMS.Application.Devices.DTOs;
using BMS.Domain.Entities.BMS;
using BMS.Domain.Entities.Location;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BMS.Application.Devices.Handlers
{
    public class CreateDeviceCommandHandler : IRequestHandler<CreateDeviceCommand, DeviceDto>
    {
        private readonly IDeviceRepository _repository;

        public CreateDeviceCommandHandler(IDeviceRepository repository)
        {
            _repository = repository;
        }

        public async Task<DeviceDto> Handle(CreateDeviceCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Device;

            //var device = new Device(dto.ControllerId, dto.Code, dto.Name, dto.Type);
            var location = new LocationReference(
                dto.SiteId,
                dto.BuildingId,
                dto.FloorId,
                dto.WardId,
                dto.RoomId);

            var device = new Device(
                dto.ControllerId,
                dto.Code,
                dto.Name,
                dto.Type,
                location);

            await _repository.AddAsync(device);
            await _repository.SaveChangesAsync();

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