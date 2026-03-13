using MediatR;
using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Exceptions;
using BMS.Domain.Exceptions;
using BMS.Application.Controllers.Commands; // شامل UpdateControllerCommand

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
            // 1. دریافت موجودیت
            // فرض می‌کنیم GetByIdAsync شما در IControllerRepository پیاده‌سازی شده است.
            var controller = await _controllerRepository
                .GetByIdAsync(request.ControllerId, cancellationToken);

            if (controller is null)
                throw new NotFoundException(
                    $"Controller with id '{request.ControllerId}' not found.");

            // 2. نرمال‌سازی کدی که از Command آمده
            var normalizedNewCode = request.Name
                .Trim()
                .ToUpperInvariant();

            // 3. بررسی تکراری بودن کد جدید
            // ⚠️ توجه: برای تکمیل این بخش، باید در IControllerRepository و پیاده‌سازی آن،
            // متدی برای بررسی وجود کد جدید با قابلیت نادیده گرفتن ID فعلی پیاده‌سازی شود.
            // برای سادگی، از متدی فرضی به نام ExistsByCodeAndIgnoringIdAsync استفاده می‌کنیم که معادل
            // ExistsByUserNameAsync در مثال کاربر است.
            var exists = await _controllerRepository
                .ExistsByCodeAndIgnoringIdAsync(
                    normalizedNewCode,
                    cancellationToken,
                    request.ControllerId); // ارسال ID برای نادیده گرفتن خود این رکورد

            if (exists)
                throw new BusinessRuleException("Controller code (Name) already exists.");

            // 4. اعمال تغییرات بر روی موجودیت با استفاده از متد جدید
            controller.UpdateInfo(
                newName: normalizedNewCode, // ارسال مقدار نرمال شده
                newTimeoutMs: request.TimeoutMs,
                newRetryCount: request.RetryCount,
                newScanIntervalMs: request.ScanIntervalMs,
                newDescription: request.Description
            );

            // 5. ذخیره تغییرات
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
