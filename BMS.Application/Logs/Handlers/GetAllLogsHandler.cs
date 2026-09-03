using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Pagination;
using BMS.Application.Logs.Queries;
using BMS.Domain.Entities.Logs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BMS.Application.Logs.Handlers
{
    public class GetAllLogsHandler
        : IRequestHandler<GetAllLogsQuery, PagedResult<Log>>
    {
        private readonly ILogRepository _logRepository;

        public GetAllLogsHandler(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<PagedResult<Log>> Handle(
            GetAllLogsQuery request,
            CancellationToken cancellationToken)
        {
            var logs = _logRepository.Logs
                .Where(x => x.ParentLogId == null)
                .OrderByDescending(x => x.LogDate);

            if (request.UserId.HasValue)
                logs = logs.Where(x => x.UserId == request.UserId)
                    .OrderByDescending(x => x.LogDate);

            if (!string.IsNullOrWhiteSpace(request.EventType) &&
                System.Enum.TryParse<EventType>(request.EventType, true, out var eventType))
            {
                logs = logs.Where(x => x.EventType == eventType)
                    .OrderByDescending(x => x.LogDate);
            }

            var result = await logs.ToPagedResultAsync(
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            await _logRepository.EnrichAsync(result.Items, cancellationToken);
            return result;
        }
    }
}
