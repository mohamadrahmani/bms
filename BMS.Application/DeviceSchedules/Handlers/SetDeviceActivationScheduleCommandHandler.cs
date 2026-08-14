using BMS.Application.Common.Exceptions;
using BMS.Application.Common.Interfaces;
using BMS.Application.DeviceSchedules.Commands;
using BMS.Application.Models;
using MediatR;

namespace BMS.Application.DeviceSchedules.Handlers
{
    public class SetDeviceActivationScheduleCommandHandler : IRequestHandler<SetDeviceActivationScheduleCommand, ApiResponse<bool>>
    {
        private readonly IDeviceScheduleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public SetDeviceActivationScheduleCommandHandler(IDeviceScheduleRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(SetDeviceActivationScheduleCommand request, CancellationToken cancellationToken)
        {
            var schedule = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (schedule is null)
                throw new NotFoundException($"Schedule with id '{request.Id}' not found.");

            // استفاده از متدی که قبلاً در کلاس Entity تعریف کردیم
            schedule.Update(
                schedule.RegisterIndex, // ثابت می‌ماند
                schedule.IsActive,      // ثابت می‌ماند
                request.StartDay,
                request.StartTime,
                request.EndDay,
                request.EndTime,
                schedule.DeviceId       // ثابت می‌ماند
            );

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ApiResponse<bool>.SuccessResponse(true, "Schedule updated successfully");
        }
    }

}
