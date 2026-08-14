using BMS.Application.Common.Exceptions;
using BMS.Application.Common.Interfaces;
using BMS.Application.PreventiveMaintenance.Commands;
using BMS.Domain.Entities.PreventiveMaintenance;
using MediatR;

namespace BMS.Application.PreventiveMaintenance.Handlers;

public sealed class AddPmScheduleAttachmentCommandHandler
    : IRequestHandler<AddPmScheduleAttachmentCommand, Guid>
{
    private readonly IPmRepository _pmRepository;
    private readonly IPmFilePolicy _filePolicy;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public AddPmScheduleAttachmentCommandHandler(
        IPmRepository pmRepository,
        IPmFilePolicy filePolicy,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _pmRepository = pmRepository;
        _filePolicy = filePolicy;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(AddPmScheduleAttachmentCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _pmRepository.GetScheduleByIdAsync(request.PmScheduleId, cancellationToken)
            ?? throw new NotFoundException("PM schedule was not found.");

        if (!schedule.IsActive)
            throw new ConflictException("Attachments cannot be added to a closed PM schedule.");

        ValidateFile(request.FileName, request.ContentType, request.FileContent.LongLength);

        var safeFileName = Path.GetFileName(request.FileName);
        var attachment = PmAttachment.ForSchedule(
            schedule.Id,
            safeFileName,
            request.ContentType,
            Path.GetExtension(safeFileName),
            request.FileContent,
            request.Description,
            RequireCurrentUser());

        await _pmRepository.AddAttachmentAsync(attachment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return attachment.Id;
    }

    private void ValidateFile(string fileName, string contentType, long fileSize)
    {
        var error = _filePolicy.Validate(fileName, contentType, fileSize);
        if (error is not null)
            throw new BusinessRuleException(error);
    }

    private Guid RequireCurrentUser() => _currentUser.UserId
        ?? throw new BusinessRuleException("Authenticated user information is unavailable.");
}

public sealed class AddPmHistoryAttachmentCommandHandler
    : IRequestHandler<AddPmHistoryAttachmentCommand, Guid>
{
    private readonly IPmRepository _pmRepository;
    private readonly IPmFilePolicy _filePolicy;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public AddPmHistoryAttachmentCommandHandler(
        IPmRepository pmRepository,
        IPmFilePolicy filePolicy,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _pmRepository = pmRepository;
        _filePolicy = filePolicy;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(AddPmHistoryAttachmentCommand request, CancellationToken cancellationToken)
    {
        var history = await _pmRepository.GetHistoryByIdAsync(request.PmServiceHistoryId, cancellationToken)
            ?? throw new NotFoundException("PM service history was not found.");

        ValidateFile(request.FileName, request.ContentType, request.FileContent.LongLength);

        var safeFileName = Path.GetFileName(request.FileName);
        var attachment = PmAttachment.ForHistory(
            history.Id,
            safeFileName,
            request.ContentType,
            Path.GetExtension(safeFileName),
            request.FileContent,
            request.Description,
            RequireCurrentUser());

        await _pmRepository.AddAttachmentAsync(attachment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return attachment.Id;
    }

    private void ValidateFile(string fileName, string contentType, long fileSize)
    {
        var error = _filePolicy.Validate(fileName, contentType, fileSize);
        if (error is not null)
            throw new BusinessRuleException(error);
    }

    private Guid RequireCurrentUser() => _currentUser.UserId
        ?? throw new BusinessRuleException("Authenticated user information is unavailable.");
}
