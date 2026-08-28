using BMS.Application.Common.Interfaces;
using BMS.Application.Models;
using BMS.Application.Pm.Commands;
using BMS.Domain.Entities.BMS;
using MediatR;

namespace BMS.Application.Pm.Handlers
{
    public class CreatePmCommandHandler
        : IRequestHandler<CreatePmCommand, ApiResponse<Guid>>
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IPmRepository _pmRepository;

        public CreatePmCommandHandler(
            IDeviceRepository deviceRepository,
            IPmRepository pmRepository)
        {
            _deviceRepository = deviceRepository;
            _pmRepository = pmRepository;
        }

        public async Task<ApiResponse<Guid>> Handle(
            CreatePmCommand request,
            CancellationToken cancellationToken)
        {
            // 1. بررسی وجود Device
            var deviceExists =
                await _deviceRepository.ExistsAsync(request.DeviceId);

            if (!deviceExists)
            {
                return ApiResponse<Guid>.FailResponse(null, "Device not found.");
            }

            // 2. بررسی PM فعال
            var hasActivePm =
                await _pmRepository.HasActivePmAsync(
                    request.DeviceId, request.Tag);

            if (hasActivePm)
            {
                return ApiResponse<Guid>.FailResponse(null, "This device already has an active PM.");
            }

            // 3. اعتبارسنجی عنوان
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return ApiResponse<Guid>.FailResponse(null,                    "PM title is required.");
            }

            // 4. اعتبارسنجی WarningDays
            if (request.WarningDays < 0)
            {
                return ApiResponse<Guid>.FailResponse(null,                    "Warning days cannot be negative.");
            }

            // 5. ایجاد PM
            var pm = new PmSchedule(
                request.DeviceId,
                request.Title,
                request.Tag,
                request.Description,
                request.DueDate,
                request.WarningDays);

            // 6. ذخیره
            await _pmRepository.AddAsync(pm);

            await _pmRepository.SaveChangesAsync();

            // 7. نتیجه
            return ApiResponse<Guid>.SuccessResponse(
                pm.Id,
                "PM created successfully.");
        }
    }
}