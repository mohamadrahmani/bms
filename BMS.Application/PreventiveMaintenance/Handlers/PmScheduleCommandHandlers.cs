using BMS.Application.Common.Exceptions;
using BMS.Application.Common.Interfaces;
using BMS.Application.PreventiveMaintenance.Commands;
using BMS.Domain.Entities.PreventiveMaintenance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BMS.Application.PreventiveMaintenance.Handlers;

public sealed class CreatePmScheduleCommandHandler
    : IRequestHandler<CreatePmScheduleCommand, Guid>
{
    private readonly IPmRepository _pmRepository;
    private readonly IDeviceRepository _deviceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreatePmScheduleCommandHandler(
        IPmRepository pmRepository,
        IDeviceRepository deviceRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _pmRepository = pmRepository;
        _deviceRepository = deviceRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreatePmScheduleCommand request, CancellationToken cancellationToken)
    {
        var userId = RequireCurrentUser();

        if (!await _deviceRepository.ExistsAsync(request.DeviceId))
            throw new NotFoundException("Device was not found.");

        if (await _pmRepository.HasActiveScheduleAsync(request.DeviceId, cancellationToken))
            throw new ConflictException("The device already has an active PM schedule.");

        var schedule = new PmSchedule(
            request.DeviceId,
            request.Title,
            request.Description,
            request.DueDateUtc,
            request.WarningDays,
            userId);

        await _pmRepository.AddScheduleAsync(schedule, cancellationToken);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            throw new ConflictException("The device already has an active PM schedule.");
        }

        return schedule.Id;
    }

    private Guid RequireCurrentUser()
    {
        return _currentUser.UserId
            ?? throw new BusinessRuleException("Authenticated user information is unavailable.");
    }
}

public sealed class UpdatePmScheduleCommandHandler
    : IRequestHandler<UpdatePmScheduleCommand>
{
    private readonly IPmRepository _pmRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdatePmScheduleCommandHandler(
        IPmRepository pmRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _pmRepository = pmRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(UpdatePmScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _pmRepository.GetScheduleByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("PM schedule was not found.");

        EnsureCanChange(schedule.IsActive, schedule.RowVersion, request.RowVersion);

        schedule.Update(
            request.Title,
            request.Description,
            request.DueDateUtc,
            request.WarningDays,
            RequireCurrentUser());

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException("The PM schedule was changed by another request. Reload and try again.");
        }

        return Unit.Value;
    }

    private Guid RequireCurrentUser()
    {
        return _currentUser.UserId
            ?? throw new BusinessRuleException("Authenticated user information is unavailable.");
    }

    private static void EnsureCanChange(bool isActive, byte[] currentRowVersion, byte[] requestedRowVersion)
    {
        if (!isActive)
            throw new ConflictException("Closed PM schedules cannot be changed.");

        if (!currentRowVersion.SequenceEqual(requestedRowVersion))
            throw new ConflictException("The PM schedule was changed by another request. Reload and try again.");
    }
}

public sealed class FinalizePmScheduleCommandHandler
    : IRequestHandler<FinalizePmScheduleCommand, Guid>
{
    private readonly IPmRepository _pmRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly TimeProvider _timeProvider;

    public FinalizePmScheduleCommandHandler(
        IPmRepository pmRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        TimeProvider timeProvider)
    {
        _pmRepository = pmRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _timeProvider = timeProvider;
    }

    public async Task<Guid> Handle(FinalizePmScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _pmRepository.GetScheduleByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("PM schedule was not found.");

        if (!schedule.IsActive)
            throw new ConflictException("The PM schedule has already been finalized.");

        if (!schedule.RowVersion.SequenceEqual(request.RowVersion))
            throw new ConflictException("The PM schedule was changed by another request. Reload and try again.");

        var currentUserId = _currentUser.UserId
            ?? throw new BusinessRuleException("Authenticated user information is unavailable.");

        var history = schedule.Finalize(
            request.Status,
            request.ActionDateUtc,
            request.Description,
            request.PerformedByUserId ?? currentUserId,
            currentUserId,
            _timeProvider.GetUtcNow().UtcDateTime);

        await _pmRepository.AddHistoryAsync(history, cancellationToken);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException("The PM schedule was finalized by another request.");
        }
        catch (DbUpdateException)
        {
            throw new ConflictException("The PM schedule has already been finalized.");
        }

        return history.Id;
    }
}
