using BMS.Application.Common.Interfaces;
using BMS.Application.Devices.Commands;
using BMS.Application.Devices.DTOs;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BMS.Application.Devices.Handlers
{
    public class UpdateDeviceCommandHandler : IRequestHandler<UpdateDeviceCommand, DeviceDto>
    {
        private readonly IDeviceRepository _repository;

        public UpdateDeviceCommandHandler(IDeviceRepository repository)
        {
            _repository = repository;
        }

        public async Task<DeviceDto> Handle(UpdateDeviceCommand request, CancellationToken cancellationToken)
            {
            var device = await _repository.GetByIdAsync(request.Id);

            if (device == null)
                throw new Exception("Device not found");

            // ---- Core fields ----
            device.Rename(request.Name);
            device.SetDescription(request.Description);

            // ---- Changeable identifiers ----
            if (device.Code != request.Code)
                device.ChangeCode(request.Code);

            if (device.Type != request.Type)
                device.ChangeType(request.Type);

            if (device.ControllerId != request.ControllerId)
                device.ChangeController(request.ControllerId);

            // ---- Settings ----
            device.SetAlarming(request.EnableAlarming);
            device.SetTrending(request.EnableTrending);

            if (request?.IsActive ?? false)
                device.Enable();
            else
                device.Disable();

            // ---- Location ----
            if (request.SiteId.HasValue &&
                request.BuildingId.HasValue &&
                request.FloorId.HasValue &&
                request.WardId.HasValue &&
                request.RoomId.HasValue)
            {
                device.UpdateLocation(
                    request.SiteId.Value,
                    request.BuildingId.Value,
                    request.FloorId.Value,
                    request.WardId.Value,
                    request.RoomId.Value);
            }

            await _repository.UpdateAsync(device);
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
                SiteId = device.Location?.SiteId,
                BuildingId = device.Location?.BuildingId,
                FloorId = device.Location?.FloorId,
                WardId = device.Location?.WardId,
                RoomId = device.Location?.RoomId,
                Description = device.Description
            };
        }
    }
}
