using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Pagination;
using BMS.Application.SystemErrorLogs.Models;
using BMS.Application.SystemErrorLogs.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BMS.Application.SystemErrorLogs.Handlers;

public sealed class GetSystemErrorLogsHandler
    : IRequestHandler<GetSystemErrorLogsQuery, PagedResult<SystemErrorLogListItemDto>>
{
    private readonly ISystemErrorLogRepository _repository;

    public GetSystemErrorLogsHandler(ISystemErrorLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<SystemErrorLogListItemDto>> Handle(
        GetSystemErrorLogsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _repository.Logs
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (request.FromUtc.HasValue)
            query = query.Where(x => x.OccurredAtUtc >= request.FromUtc.Value);

        if (request.ToUtc.HasValue)
            query = query.Where(x => x.OccurredAtUtc <= request.ToUtc.Value);

        if (request.IsResolved.HasValue)
            query = query.Where(x => x.IsResolved == request.IsResolved.Value);

        if (request.StatusCode.HasValue)
            query = query.Where(x => x.StatusCode == request.StatusCode.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x =>
                x.Message.Contains(search) ||
                x.ExceptionType.Contains(search) ||
                (x.RequestPath != null && x.RequestPath.Contains(search)) ||
                x.ErrorId.ToString().Contains(search));
        }

        var ordered = query
            .OrderByDescending(x => x.OccurredAtUtc)
            .Select(x => new SystemErrorLogListItemDto
            {
                Id = x.Id,
                ErrorId = x.ErrorId,
                OccurredAtUtc = x.OccurredAtUtc,
                ExceptionType = x.ExceptionType,
                Message = x.Message,
                RequestPath = x.RequestPath,
                StatusCode = x.StatusCode,
                UserId = x.UserId,
                IsResolved = x.IsResolved
            });

        return await ordered.ToPagedResultAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}
