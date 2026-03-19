
using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Exceptions;
using BMS.Application.Devices.Commands;
using BMS.Domain.Entities.BMS;
using BMS.Domain.Entities.Location;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BMS.Application.Devices.Handlers
{
    public class CreateDeviceCommandHandler : IRequestHandler<CreateDeviceCommand, Guid>
    {
        private readonly IDeviceRepository _repository;

        public CreateDeviceCommandHandler(IDeviceRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateDeviceCommand request, CancellationToken cancellationToken)
        {
            // ✅ ساخت Location (اختیاری)
            LocationReference? location = null;

            if (request.SiteId.HasValue || request.BuildingId.HasValue ||
                request.FloorId.HasValue || request.WardId.HasValue || request.RoomId.HasValue)
            {
                location = new LocationReference(
                    request.SiteId,
                    request.BuildingId,
                    request.FloorId,
                    request.WardId,
                    request.RoomId
                    );
            }

            // ✅ ساخت Device
            var device = new Device(
                request.ControllerId,
                request.Code.Trim().ToUpperInvariant(),
                request.Name,
                request.Type,
                request.Description,
                request.IsActive,
                location
            );

           // device.SetActive(request.IsActive);

            // ✅ ذخیره در دیتابیس
            await _repository.AddAsync(device);
            await _repository.SaveChangesAsync();

            // ✅ برگرداندن Id
            return device.Id;
        }
    }
}
