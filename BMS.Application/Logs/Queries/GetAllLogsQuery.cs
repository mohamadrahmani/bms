
using BMS.Application.Common.Pagination;
using BMS.Domain.Entities.Logs;
using MediatR;

namespace BMS.Application.Logs.Queries
{
    //public record GetAllLogsQuery() : PagedRequest, IRequest<PagedResult<Log>>;
    public sealed class GetAllLogsQuery
       : PagedRequest, IRequest<PagedResult<Log>>
    {
        // می‌توانی فیلتر اضافه کنی later:
        public Guid? UserId { get; set; }
        public string? EventType { get; set; }
    }
}
