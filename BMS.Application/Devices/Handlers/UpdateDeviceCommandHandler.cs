using BMS.Application.Common.Interfaces;
using BMS.Application.Devices.Commands;
using BMS.Application.Devices.DTOs;
using BMS.Application.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BMS.Application.Devices.Handlers
{
    public class UpdateDeviceCommandHandler : IRequestHandler<UpdateDeviceCommand, ApiResponse<bool>>
    {
        private readonly IDeviceRepository _repository;

        public UpdateDeviceCommandHandler(IDeviceRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateDeviceCommand request, CancellationToken cancellationToken)
        {
            var device = await _repository.GetByIdAsync(request.Id);

            if (device == null)
                throw new Exception("Device not found");
            //***********************************
            request.Changes ??= new();

            void Track(string field, object? oldValue, object? newValue)
            {
                if (oldValue?.ToString() != newValue?.ToString())
                    request.Changes.Add((field, oldValue?.ToString(), newValue?.ToString()));
            }

            // --------------------------
            // مقایسه و TrackChange
            // --------------------------

            Track("Name", device.Name, request.Name);
            Track("Description", device.Description, request.Description);
            Track("Code", device.Code, request.Code);
            Track("Type", device.Type, request.Type);

            Track("ControllerId", device.ControllerId, request.ControllerId);

            Track("EnableAlarming", device.EnableAlarming, request.EnableAlarming);
            Track("EnableTrending", device.EnableTrending, request.EnableTrending);
            Track("IsActive", device.IsActive, request.IsActive);

            // Location
            var loc = device.Location;

            Track("SiteId", loc?.SiteId, request.SiteId);
            Track("BuildingId", loc?.BuildingId, request.BuildingId);
            Track("FloorId", loc?.FloorId, request.FloorId);
            Track("WardId", loc?.WardId, request.WardId);
            Track("RoomId", loc?.RoomId, request.RoomId);
            // --------------------------
            // اعمال تغییرات
            // --------------------------

            if (device.Name != request.Name)
                device.Rename(request.Name);

            if (device.Description != request.Description)
                device.SetDescription(request.Description);

            if (device.Code != request.Code)
                device.ChangeCode(request.Code);

            if (device.Type != request.Type)
                device.ChangeType(request.Type);

            if (device.ControllerId != request.ControllerId)
                device.ChangeController(request.ControllerId);

            device.SetAlarming(request.EnableAlarming);
            device.SetTrending(request.EnableTrending);

            if (request.IsActive == true)
                device.Enable();
            else if (request.IsActive == false)
                device.Disable();
            //***********************************
            //// ---- Core fields ----
            //device.Rename(request.Name);
            //device.SetDescription(request.Description);

            //// ---- Changeable identifiers ----
            //if (device.Code != request.Code)
            //    device.ChangeCode(request.Code);

            //if (device.Type != request.Type)
            //    device.ChangeType(request.Type);

            //if (device.ControllerId != request.ControllerId)
            //    device.ChangeController(request.ControllerId);

            //// ---- Settings ----
            //device.SetAlarming(request.EnableAlarming);
            //device.SetTrending(request.EnableTrending);

            //if (request?.IsActive ?? false)
            //    device.Enable();
            //else
            //    device.Disable();

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

            return ApiResponse<bool>.SuccessResponse(true, "دستگاه با موفقیت به روزرسانی شد");

            //return new DeviceDto
            //{
            //    Id = device.Id,
            //    ControllerId = device.ControllerId,
            //    Code = device.Code,
            //    Name = device.Name,
            //    Type = device.Type,
            //    EnableAlarming = device.EnableAlarming,
            //    EnableTrending = device.EnableTrending,
            //    IsActive = device.IsActive,
            //    SiteId = device.Location?.SiteId,
            //    BuildingId = device.Location?.BuildingId,
            //    FloorId = device.Location?.FloorId,
            //    WardId = device.Location?.WardId,
            //    RoomId = device.Location?.RoomId,
            //    Description = device.Description
            //};
        }
    }
}
