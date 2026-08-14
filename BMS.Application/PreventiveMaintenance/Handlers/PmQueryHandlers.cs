using BMS.Application.Common.Exceptions;
using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Pagination;
using BMS.Application.PreventiveMaintenance.Dtos;
using BMS.Application.PreventiveMaintenance.Queries;
using BMS.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BMS.Application.PreventiveMaintenance.Handlers;

public sealed class GetActivePmByDeviceQueryHandler
    : IRequestHandler<GetActivePmByDeviceQuery, ActivePmDto>
{
    private readonly IPmRepository _pmRepository;
    private readonly IDeviceRepository _deviceRepository;
    private readonly TimeProvider _timeProvider;

    public GetActivePmByDeviceQueryHandler(
        IPmRepository pmRepository,
        IDeviceRepository deviceRepository,
        TimeProvider timeProvider)
    {
        _pmRepository = pmRepository;
        _deviceRepository = deviceRepository;
        _timeProvider = timeProvider;
    }

    public async Task<ActivePmDto> Handle(GetActivePmByDeviceQuery request, CancellationToken cancellationToken)
    {
        if (!await _deviceRepository.ExistsAsync(request.DeviceId))
            throw new NotFoundException("Device was not found.");

        var schedule = await _pmRepository.Schedules
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.DeviceId == request.DeviceId && x.IsActive && !x.IsDeleted,
                cancellationToken);

        if (schedule is null)
        {
            return new ActivePmDto
            {
                HasActivePm = false,
                DeviceId = request.DeviceId,
                Indicator = PmIndicatorStatus.None
            };
        }

        var attachments = await _pmRepository.Attachments
            .AsNoTracking()
            .Where(x => x.PmScheduleId == schedule.Id && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new PmAttachmentDto
            {
                Id = x.Id,
                FileName = x.FileName,
                ContentType = x.ContentType,
                FileExtension = x.FileExtension,
                FileSize = x.FileSize,
                Description = x.Description,
                CreatedAtUtc = x.CreatedAtUtc,
                CreatedByUserId = x.CreatedByUserId
            })
            .ToListAsync(cancellationToken);

        var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;
        return new ActivePmDto
        {
            HasActivePm = true,
            PmScheduleId = schedule.Id,
            DeviceId = schedule.DeviceId,
            Title = schedule.Title,
            Description = schedule.Description,
            DueDateUtc = schedule.DueDateUtc,
            WarningDays = schedule.WarningDays,
            Indicator = schedule.CalculateIndicator(nowUtc),
            DaysUntilDue = (int)Math.Ceiling((schedule.DueDateUtc - nowUtc).TotalDays),
            RowVersion = schedule.RowVersion,
            Attachments = attachments
        };
    }
}

public sealed class GetPmHistoryByDeviceQueryHandler
    : IRequestHandler<GetPmHistoryByDeviceQuery, PagedResult<PmHistoryDto>>
{
    private readonly IPmRepository _pmRepository;
    private readonly IDeviceRepository _deviceRepository;

    public GetPmHistoryByDeviceQueryHandler(
        IPmRepository pmRepository,
        IDeviceRepository deviceRepository)
    {
        _pmRepository = pmRepository;
        _deviceRepository = deviceRepository;
    }

    public async Task<PagedResult<PmHistoryDto>> Handle(
        GetPmHistoryByDeviceQuery request,
        CancellationToken cancellationToken)
    {
        if (!await _deviceRepository.ExistsAsync(request.DeviceId))
            throw new NotFoundException("Device was not found.");

        var query = _pmRepository.Histories
            .AsNoTracking()
            .Where(x => x.PmSchedule.DeviceId == request.DeviceId && !x.IsDeleted)
            .OrderByDescending(x => x.ActionDateUtc);

        var totalCount = await query.CountAsync(cancellationToken);
        var rows = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new PmHistoryDto
            {
                Id = x.Id,
                PmScheduleId = x.PmScheduleId,
                Status = x.Status,
                ActionDateUtc = x.ActionDateUtc,
                Description = x.Description,
                PerformedByUserId = x.PerformedByUserId,
                CreatedAtUtc = x.CreatedAtUtc,
                CreatedByUserId = x.CreatedByUserId,
                DueDateUtcSnapshot = x.DueDateUtcSnapshot,
                TitleSnapshot = x.TitleSnapshot,
                BaseDescriptionSnapshot = x.BaseDescriptionSnapshot
            })
            .ToListAsync(cancellationToken);

        var historyIds = rows.Select(x => x.Id).ToArray();
        var attachmentRows = await _pmRepository.Attachments
            .AsNoTracking()
            .Where(x => x.PmServiceHistoryId.HasValue &&
                        historyIds.Contains(x.PmServiceHistoryId.Value) &&
                        !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new
            {
                HistoryId = x.PmServiceHistoryId!.Value,
                Attachment = new PmAttachmentDto
                {
                    Id = x.Id,
                    FileName = x.FileName,
                    ContentType = x.ContentType,
                    FileExtension = x.FileExtension,
                    FileSize = x.FileSize,
                    Description = x.Description,
                    CreatedAtUtc = x.CreatedAtUtc,
                    CreatedByUserId = x.CreatedByUserId
                }
            })
            .ToListAsync(cancellationToken);

        var attachments = attachmentRows
            .GroupBy(x => x.HistoryId)
            .ToDictionary(x => x.Key, x => (IReadOnlyList<PmAttachmentDto>)x.Select(a => a.Attachment).ToList());

        var items = rows.Select(x => new PmHistoryDto
        {
            Id = x.Id,
            PmScheduleId = x.PmScheduleId,
            Status = x.Status,
            ActionDateUtc = x.ActionDateUtc,
            Description = x.Description,
            PerformedByUserId = x.PerformedByUserId,
            CreatedAtUtc = x.CreatedAtUtc,
            CreatedByUserId = x.CreatedByUserId,
            DueDateUtcSnapshot = x.DueDateUtcSnapshot,
            TitleSnapshot = x.TitleSnapshot,
            BaseDescriptionSnapshot = x.BaseDescriptionSnapshot,
            Attachments = attachments.GetValueOrDefault(x.Id, Array.Empty<PmAttachmentDto>())
        }).ToList();

        return new PagedResult<PmHistoryDto>(items, totalCount, request.PageNumber, request.PageSize);
    }
}

public sealed class DownloadPmAttachmentQueryHandler
    : IRequestHandler<DownloadPmAttachmentQuery, PmAttachmentDownloadDto>
{
    private readonly IPmRepository _pmRepository;

    public DownloadPmAttachmentQueryHandler(IPmRepository pmRepository)
    {
        _pmRepository = pmRepository;
    }

    public async Task<PmAttachmentDownloadDto> Handle(
        DownloadPmAttachmentQuery request,
        CancellationToken cancellationToken)
    {
        var attachment = await _pmRepository.Attachments
            .AsNoTracking()
            .Where(x => x.Id == request.AttachmentId && !x.IsDeleted)
            .Select(x => new PmAttachmentDownloadDto
            {
                FileName = x.FileName,
                ContentType = x.ContentType,
                Content = x.FileContent
            })
            .FirstOrDefaultAsync(cancellationToken);

        return attachment ?? throw new NotFoundException("PM attachment was not found.");
    }
}
