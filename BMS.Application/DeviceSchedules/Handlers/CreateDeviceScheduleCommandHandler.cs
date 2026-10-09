using BMS.Application.Common.Interfaces;
using BMS.Application.DeviceSchedules.Commands;
using BMS.Application.Models;
using BMS.Domain.Entities.BMS;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BMS.Application.Interfaces;
using BMS.Domain.Events;
using BMS.Application.UseCases;

namespace BMS.Application.DeviceSchedules.Handlers;

public sealed class CreateDeviceScheduleCommandHandler: IRequestHandler<CreateDeviceScheduleCommand, ApiResponse<Guid>>
{
    private readonly IDeviceScheduleRepository _deviceScheduleRepository;
    private readonly IPointRepository _pointRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ExecuteCommandUseCase _useCase;
    private readonly IEventDispatcher _eventDispatcher;

    public CreateDeviceScheduleCommandHandler(
        IDeviceScheduleRepository deviceScheduleRepository,
        IPointRepository pointRepository,
        IUnitOfWork unitOfWork,
        ExecuteCommandUseCase useCase,
        IEventDispatcher eventDispatcher)
    {
        _deviceScheduleRepository = deviceScheduleRepository;
        _pointRepository = pointRepository;
        _unitOfWork = unitOfWork;
        _useCase = useCase;
        _eventDispatcher = eventDispatcher;


    }

    public async Task<ApiResponse<Guid>> Handle(
        CreateDeviceScheduleCommand request,
        CancellationToken cancellationToken)
    {
        var deviceSchedule = new DeviceSchedule()
        {
            Id = Guid.NewGuid(),
            DeviceId = request.DeviceId,
            StartDay = request.StartDay,
            StartTime = request.StartTime,
            EndDay = request.EndDay,
            IsActive = request.IsActive,
            EndTime = request.EndTime,
            RegisterIndex = request.RegisterIndex
        };



        var pointTime = await _pointRepository.Points.FirstOrDefaultAsync(p => p.DeviceId == request.DeviceId && p.Tag == "Main_Time_Schedule_First_Register");

        if (pointTime == null || pointTime != null && pointTime.Address == null)
        {

            if (pointTime == null)
            {
                pointTime = new Point(request.DeviceId, PointKind.AI, null, "Main_Time_Schedule_First_Register",
                              "آدرس شروع گروه ساعت زمانبندی",
                              PointDataType.UInt16, PointType.SetPoint, RegisterType.HoldingRegister, isWritable: true);

                await _pointRepository.AddAsync(pointTime);
            }

            throw new Exception("آدرس شروع گروه ساعت زمانبندی ثبت نشده است (مقدار آدرس رجیستر Main_Time_Schedule_First_Register را تعیین کنید).");
        }

        var pointTimeEnable = await _pointRepository.Points.FirstOrDefaultAsync(p => p.DeviceId == request.DeviceId && p.Tag == "Main_1St_Time_Schedule_Enable");

        if (pointTimeEnable == null)
        {
            if (pointTimeEnable == null)
            {
                pointTimeEnable = new Point(request.DeviceId, PointKind.AI, null, "Main_1St_Time_Schedule_Enable",
                              "آدرس شروع گروه تنظیمات فعال/غیرفعال زمانبندی",
                              PointDataType.Boolean, PointType.SetPoint, RegisterType.Coil, isWritable: true);

                await _pointRepository.AddAsync(pointTimeEnable);
            }

            throw new Exception("آدرس شروع گروه تنظیمات فعال/غیرفعال زمانبندی ثبت نشده است (مقدار آدرس رجیستر Main_1St_Time_Schedule_Enable را تعیین کنید)");
        }

        await _deviceScheduleRepository.AddAsync(deviceSchedule, cancellationToken);


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

        var p1 = await _pointRepository.Points.FirstOrDefaultAsync(p => p.DeviceId == request.DeviceId && p.Tag == "Time_Schedule_Enable_" + request.RegisterIndex.ToString());

        if(p1 == null)
        {

            p1 = new Point(request.DeviceId, PointKind.AI, (ushort)(pointTimeEnable.Address + request.RegisterIndex), "Time_Schedule_Enable_" + request.RegisterIndex.ToString(),
                          "Time_Schedule_Enable_" + request.RegisterIndex.ToString(),
                          PointDataType.Boolean, PointType.Command, RegisterType.Coil, isWritable: true);

            await _pointRepository.AddAsync(p1);
        }

        var p2 = await _pointRepository.Points.FirstOrDefaultAsync(p => p.DeviceId == request.DeviceId && p.Tag == "Time_Schedule_Start_" + request.RegisterIndex.ToString());

        if (p2 == null)
        {
            p2 = new Point(request.DeviceId, PointKind.AI, (ushort)(request.RegisterIndex * 2 + pointTime.Address), "Time_Schedule_Start_" + request.RegisterIndex.ToString(),
                          "Time_Schedule_Start_" + request.RegisterIndex.ToString(),
                          PointDataType.Boolean, PointType.Command, RegisterType.Coil, isWritable: true);

            await _pointRepository.AddAsync(p2);
        }

        var p3 = await _pointRepository.Points.FirstOrDefaultAsync(p => p.DeviceId == request.DeviceId && p.Tag == "Time_Schedule_End_" + request.RegisterIndex.ToString());

        if (p3 == null)
        {
            p3 = new Point(request.DeviceId, PointKind.AI, (ushort)(request.RegisterIndex * 2 + pointTime.Address + 1), "Time_Schedule_End_" + request.RegisterIndex.ToString(),
                          "Time_Schedule_End_" + request.RegisterIndex.ToString(),
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

        var command = new SensorCommand();
        var commandId = await _useCase.ExecuteAsync(
        request.DeviceId,
        command.CommandName,
        p1.Id,
        request.IsActive ? "1" : "0" );

        await _eventDispatcher.DispatchAsync(
        new DeviceCommandCompletedDomainEvent(
            Guid.Empty,
            command.DeviceId,
            command.CommandName,
            true,
            null,
            DateTime.UtcNow)
        );

        command = new SensorCommand();
        commandId = await _useCase.ExecuteAsync(
        request.DeviceId,
        command.CommandName,
        p2.Id,
        ConvertToWeekMinutes(request.StartDay, request.StartTime.Value).ToString());

        await _eventDispatcher.DispatchAsync(
        new DeviceCommandCompletedDomainEvent(
            Guid.Empty,
            command.DeviceId,
            command.CommandName,
            true,
            null,
            DateTime.UtcNow)
        );

        command = new SensorCommand();
        commandId = await _useCase.ExecuteAsync(
        request.DeviceId,
        command.CommandName,
        p3.Id,
        ConvertToWeekMinutes(request.StartDay, request.StartTime.Value).ToString());

        await _eventDispatcher.DispatchAsync(
        new DeviceCommandCompletedDomainEvent(
            Guid.Empty,
            command.DeviceId,
            command.CommandName,
            true,
            null,
            DateTime.UtcNow)
        );
        return ApiResponse<Guid>.SuccessResponse(deviceSchedule.Id, "زمانبندی ثبت شد");
    }

    public static int ConvertToWeekMinutes(int dayId, TimeSpan time)
    {
        return (dayId * 24 * 60) + (time.Hours * 60) + time.Minutes;
    }
}
