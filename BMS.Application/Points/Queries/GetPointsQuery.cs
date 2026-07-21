using BMS.Application.Common.Filters;
using BMS.Application.Common.Pagination;
using BMS.Application.Points.Dtos;
using BMS.Domain.Entities.BMS;
using MediatR;

namespace BMS.Application.Points.Queries;

public class GetPointsQuery : PagedRequest, IRequest<PagedResult<PointDto>>
{
    public Guid? DeviceId { get; set; }
    public Guid? ControllerId { get; set; }

    public Guid? SiteId { get; set; }
    public Guid? BuildingId { get; set; }
    public Guid? FloorId { get; set; }
    public Guid? WardId { get; set; }
    public Guid? RoomId { get; set; }
    public List<FilterDto>? Filters { get; set; }
    public PointKind? Kind { get; set; }
    public PointDataType? DataType { get; set; }

    public string? Search { get; set; }
}
