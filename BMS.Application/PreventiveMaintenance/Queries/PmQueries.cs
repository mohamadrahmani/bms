using BMS.Application.Common.Pagination;
using BMS.Application.PreventiveMaintenance.Dtos;
using MediatR;

namespace BMS.Application.PreventiveMaintenance.Queries;

public sealed record GetActivePmByDeviceQuery(Guid DeviceId) : IRequest<ActivePmDto>;

public sealed class GetPmHistoryByDeviceQuery : PagedRequest, IRequest<PagedResult<PmHistoryDto>>
{
    public Guid DeviceId { get; set; }
}

public sealed record DownloadPmAttachmentQuery(Guid AttachmentId) : IRequest<PmAttachmentDownloadDto>;
