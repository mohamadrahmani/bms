using MediatR;
using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Exceptions;
using BMS.Domain.Exceptions;
using BMS.Application.Controllers.Commands;
using BMS.Domain.Entities.Location;
using BMS.Application.Models;

namespace BMS.Application.Controllers.Handlers
{
    public sealed class UpdateControllerCommandHandler
        : IRequestHandler<UpdateControllerCommand, ApiResponse<bool>>
    {
        private readonly IControllerRepository _controllerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateControllerCommandHandler(
            IControllerRepository controllerRepository,
            IUnitOfWork unitOfWork)
        {
            _controllerRepository = controllerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(
            UpdateControllerCommand request,
            CancellationToken cancellationToken)
        {
            // 1. دریافت Controller
            var controller = await _controllerRepository
                .GetByIdAsync(request.ControllerId, cancellationToken);

            if (controller is null)
                throw new NotFoundException(
                    $"Controller with id '{request.ControllerId}' not found.");

            // 2. نرمال سازی Code
            var normalizedCode = request.Code
                .Trim()
                .ToUpperInvariant();

            // 3. بررسی تکراری نبودن Code
            var exists = await _controllerRepository
                .ExistsByCodeAndIgnoringIdAsync(
                    normalizedCode,
                    cancellationToken,
                    request.ControllerId);

            if (exists)
                throw new BusinessRuleException("Controller code already exists.");
            //*****************
            // 3. آماده‌سازی لیست تغییرات
            request.Changes ??= new();

            void TrackChange(string name, object? oldValue, object? newValue)
            {
                string? oldStr = oldValue?.ToString();
                string? newStr = newValue?.ToString();
                if (oldStr != newStr)
                    request.Changes.Add((name, oldStr, newStr));
            }
            // 4. مقایسه فیلدها
            TrackChange("Code", controller.Code, normalizedCode);
            TrackChange("Name", controller.Name, request.Name);
            TrackChange("Protocol", controller.Protocol, request.Protocol);
            TrackChange("IpAddress", controller.IpAddress, request.IpAddress);
            TrackChange("Port", controller.Port, request.Port);
            TrackChange("UnitId", controller.UnitId, request.UnitId);
            TrackChange("TimeoutMs", controller.TimeoutMs, request.TimeoutMs);
            TrackChange("RetryCount", controller.RetryCount, request.RetryCount);
            TrackChange("ScanIntervalMs", controller.ScanIntervalMs, request.ScanIntervalMs);
            TrackChange("Description", controller.Description, request.Description);
            TrackChange("FirmwareVersion", controller.FirmwareVersion, request.FirmwareVersion);
            TrackChange("HealthStatus", controller.HealthStatus, request.HealthStatus);
            TrackChange("IsActive", controller.IsActive, request.IsActive);

            var loc = controller.Location;
            TrackChange("SiteId", loc?.SiteId, request.SiteId);
            TrackChange("BuildingId", loc?.BuildingId, request.BuildingId);
            TrackChange("FloorId", loc?.FloorId, request.FloorId);
            TrackChange("WardId", loc?.WardId, request.WardId);
            TrackChange("RoomId", loc?.RoomId, request.RoomId);
            // 4. اعمال تغییرات
            controller.UpdateInfo(
            newCode: normalizedCode,
            newName: request.Name,
            newProtocol: request.Protocol,
            newIpAddress: request.IpAddress,
            newPort: request.Port,
            newUnitId: request.UnitId,
            newTimeoutMs: request.TimeoutMs,
            newRetryCount: request.RetryCount,
            newScanIntervalMs: request.ScanIntervalMs,
            newDescription: request.Description,
            newFirmwareVersion: request.FirmwareVersion,
            newHealthStatus: request.HealthStatus,
            newIsActive: request.IsActive
        );
            var location = new LocationReference(
                request.SiteId,
                request.BuildingId,
                request.FloorId,
                request.WardId,
                request.RoomId
            );

            controller.UpdateLocation(location);

            // 6. ذخیره
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.SuccessResponse(true, "Device updated successfully");
        }
    }
}
