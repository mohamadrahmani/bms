using BMS.Application.Common.Interfaces;
using BMS.Application.Devices.Commands;
using BMS.Application.Devices.DTOs;
using MediatR;
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
            var dto = request.Device;

            var device = await _repository.GetByIdAsync(dto.Id);

            if (device == null)
                throw new Exception("Device not found");

            device.Rename(dto.Name);

            device.SetAlarming(dto.EnableAlarming);
            device.SetTrending(dto.EnableTrending);

            if (dto.IsActive)
                device.Enable();
            else
                device.Disable();
            // ✅ Update Location
            device.UpdateLocation(
                dto.SiteId,
                dto.BuildingId,
                dto.FloorId,
                dto.WardId,
                dto.RoomId);

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
                IsActive = device.IsActive
            };
        }
    }
}
