using BMS.Application.Common.Pagination;
using BMS.Application.SystemErrorLogs.Models;
using MediatR;

namespace BMS.Application.SystemErrorLogs.Queries;

public sealed class GetSystemErrorLogsQuery
    : PagedRequest, IRequest<PagedResult<SystemErrorLogListItemDto>>
{
    public DateTime? FromUtc { get; set; }
    public DateTime? ToUtc { get; set; }
    public bool? IsResolved { get; set; }
    public int? StatusCode { get; set; }
    public string? Search { get; set; }
}
