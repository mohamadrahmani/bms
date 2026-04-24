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
            var logs = _logRepository.Logs.AsQueryable();
            // Filtering
            if (request.UserId.HasValue)
                logs = logs.Where(x => x.UserId == request.UserId);

            if (!string.IsNullOrWhiteSpace(request.EventType))
                logs = logs.Where(x => x.EventType.ToString() == request.EventType);
            ///////
            var query = logs
                .OrderByDescending(x => x.LogDate);

            return await query.ToPagedResultAsync(
                request.PageNumber,
                request.PageSize,
                cancellationToken);
        }
    }
}
