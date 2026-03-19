using MediatR;
using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Exceptions;
using BMS.Domain.Exceptions;
using BMS.Application.Controllers.Commands;
using BMS.Domain.Entities.Location;

namespace BMS.Application.Controllers.Handlers
{
    public sealed class UpdateControllerCommandHandler
        : IRequestHandler<UpdateControllerCommand>
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

        public async Task<Unit> Handle(
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

            // 5. آپدیت Location
            //controller.UpdateLocation(
            //    request.SiteId,
            //    request.BuildingId,
            //    request.FloorId,
            //    request.WardId,
            //    request.RoomId
            //);

            // 6. ذخیره
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
