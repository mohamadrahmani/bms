using BMS.Application.Common.Exceptions;
using BMS.Application.Common.Interfaces;
using BMS.Application.DeviceSchedules.Commands;
using BMS.Application.Interfaces;
using BMS.Application.Models;
using BMS.Application.UseCases;
using BMS.Domain.Entities.BMS;
using BMS.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BMS.Application.DeviceSchedules.Handlers
{
    public class UpdateDeviceScheduleCommandHandler : IRequestHandler<UpdateDeviceScheduleCommand, ApiResponse<bool>>
    {
        private readonly IDeviceScheduleRepository _repository;
        private readonly IPointRepository _pointRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ExecuteCommandUseCase _useCase;
        private readonly IEventDispatcher _eventDispatcher;

        public UpdateDeviceScheduleCommandHandler(IDeviceScheduleRepository repository, IPointRepository pointRepository, IUnitOfWork unitOfWork,
            ExecuteCommandUseCase useCase, IEventDispatcher eventDispatcher)
        {
            _repository = repository;
            _pointRepository = pointRepository;
            _unitOfWork = unitOfWork;
            _useCase = useCase;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateDeviceScheduleCommand request, CancellationToken cancellationToken)
        {
            var schedule = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (schedule is null)
                throw new NotFoundException($"Schedule with id '{request.Id}' not found.");

            var pointTime = await _pointRepository.Points.FirstOrDefaultAsync(p => p.DeviceId == schedule.DeviceId && p.Tag == "Main_Time_Schedule_First_Register");

            if (pointTime == null || pointTime!= null && pointTime.Address == null)
            {

                if (pointTime == null)
                {
                    pointTime = new Point(schedule.DeviceId, PointKind.AI, null, "Main_Time_Schedule_First_Register",
                              "آدرس شروع گروه ساعت زمانبندی",
                              PointDataType.UInt16, PointType.SetPoint, RegisterType.HoldingRegister, isWritable: true);

                    await _pointRepository.AddAsync(pointTime);
                }

                throw new Exception("آدرس شروع گروه ساعت زمانبندی ثبت نشده است )مقدار آدرس رجیستر Main_Time_Schedule_First_Register را تعیین کنید.");
            }



            var pointTimeEnable = await _pointRepository.Points.FirstOrDefaultAsync(p => p.DeviceId == schedule.DeviceId && p.Tag == "Main_1St_Time_Schedule_Enable");

            if (pointTimeEnable == null)
            {
                if (pointTimeEnable == null)
                {
                    pointTimeEnable = new Point(schedule.DeviceId, PointKind.AI, null, "Main_1St_Time_Schedule_Enable",
                                  "آدرس شروع گروه تنظیمات فعال/غیرفعال زمانبندی",
                                  PointDataType.Boolean, PointType.SetPoint, RegisterType.Coil, isWritable: true);

                    await _pointRepository.AddAsync(pointTimeEnable);
                }

                throw new Exception("آدرس شروع گروه تنظیمات فعال/غیرفعال زمانبندی ثبت نشده است (مقدار آدرس رجیستر Main_1St_Time_Schedule_Enable را تعیین کنید)");
            }

            // استفاده از متدی که قبلاً در کلاس Entity تعریف کردیم
            schedule.Update(
                schedule.RegisterIndex, // ثابت می‌ماند
                request.IsActive,      // ثابت می‌ماند
                request.StartDay,
                request.StartTime,
                request.EndDay,
                request.EndTime,
                schedule.DeviceId       // ثابت می‌ماند
            );

            //// 1️⃣ Prevent duplicate DeviceSchedule Code
            //var codeExists = await _deviceScheduleRepository
            //    .ExistsByCodeAsync(request.Code, cancellationToken);

            //if (codeExists)
            //    throw new BusinessRuleException(
            //        $"DeviceSchedule with code '{request.Code}' already exists.");

            //// 2️⃣ Normalize Code
            //var normalizedCode = request.Code
            //    .Trim()
            //    .ToUpperInvariant();

            // 3️⃣ Create DeviceSchedule

            var p1 = await _pointRepository.Points.FirstOrDefaultAsync(p => p.DeviceId == schedule.DeviceId && p.Tag == "Time_Schedule_Enable_" + schedule.RegisterIndex.ToString());

            if (p1 == null)
            {

                p1 = new Point(schedule.DeviceId, PointKind.AI, (ushort)(pointTimeEnable.Address + schedule.RegisterIndex), "Time_Schedule_Enable_" + schedule.RegisterIndex.ToString(),
                              "Time_Schedule_Enable_" + schedule.RegisterIndex.ToString(),
                              PointDataType.Boolean, PointType.Command, RegisterType.Coil, isWritable: true);

                await _pointRepository.AddAsync(p1);
            }

            var p2 = await _pointRepository.Points.FirstOrDefaultAsync(p => p.DeviceId == schedule.DeviceId && p.Tag == "Time_Schedule_Start_" + schedule.RegisterIndex.ToString());

            if (p2 == null)
            {
                p2 = new Point(schedule.DeviceId, PointKind.AI, (ushort)(schedule.RegisterIndex * 2 + pointTime.Address), "Time_Schedule_Start_" + schedule.RegisterIndex.ToString(),
                              "Time_Schedule_Start_" + schedule.RegisterIndex.ToString(),
                              PointDataType.Boolean, PointType.Command, RegisterType.Coil, isWritable: true);

                await _pointRepository.AddAsync(p2);
            }

            var p3 = await _pointRepository.Points.FirstOrDefaultAsync(p => p.DeviceId == schedule.DeviceId && p.Tag == "Time_Schedule_End_" + schedule.RegisterIndex.ToString());

            if (p3 == null)
            {
                p3 = new Point(schedule.DeviceId, PointKind.AI, (ushort)(schedule.RegisterIndex * 2 + pointTime.Address + 1), "Time_Schedule_End_" + schedule.RegisterIndex.ToString(),
                              "Time_Schedule_End_" + schedule.RegisterIndex.ToString(),
                              PointDataType.Boolean, PointType.Command, RegisterType.Coil, isWritable: true);

                await _pointRepository.AddAsync(p3);
            }

            //deviceSchedule.SetActive(request.IsActive);
            //deviceSchedule.SetFirmwareVersion(request.FirmwareVersion);
            //if (request.HealthStatus.HasValue)
            //{
            //    deviceSchedule.SetHealthStatus(request.HealthStatus.Value);
            //}


            //deviceSchedule.UpdateLocation(location);
            //        deviceSchedule.UpdateLocation(
            //    request.SiteId,
            //    request.BuildingId,
            //    request.FloorId,
            //    request.WardId,
            //    request.RoomId
            //);

            // 4️⃣ Persist
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            
            var commandId = await _useCase.ExecuteAsync(schedule.DeviceId, "", p1.Id, request.IsActive ? "1" : "0");

            await _eventDispatcher.DispatchAsync(
            new DeviceCommandCompletedDomainEvent(
                Guid.Empty,
                schedule.DeviceId,
                "",
                true,
                null,
                DateTime.UtcNow)
            );

            commandId = await _useCase.ExecuteAsync(
            schedule.DeviceId,
            "",
            p2.Id,
            ConvertToWeekMinutes(request.StartDay, request.StartTime.Value).ToString());

            await _eventDispatcher.DispatchAsync(
            new DeviceCommandCompletedDomainEvent(
                Guid.Empty,
                schedule.DeviceId,
                "",
                true,
                null,
                DateTime.UtcNow)
            );

            commandId = await _useCase.ExecuteAsync(
            schedule.DeviceId,
            "",
            p3.Id,
            ConvertToWeekMinutes(request.EndDay, request.EndTime.Value).ToString().ToString());

            await _eventDispatcher.DispatchAsync(
            new DeviceCommandCompletedDomainEvent(
                Guid.Empty,
                schedule.DeviceId,
                "",
                true,
                null,
                DateTime.UtcNow)
            );

            return ApiResponse<bool>.SuccessResponse(true, "زمانبندی به روز رسانی شد");
        }
        public static int ConvertToWeekMinutes(int dayId, TimeSpan time)
        {
            return (dayId * 24 * 60) + (time.Hours * 60) + time.Minutes;
        }
    }

}
