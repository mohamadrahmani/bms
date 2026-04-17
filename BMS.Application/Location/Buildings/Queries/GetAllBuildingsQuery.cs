using MediatR;
using BMS.Application.Location.Buildings.Dtos;
using BMS.Application.Common.Pagination;

namespace BMS.Application.Location.Buildings.Queries;

public sealed class GetAllBuildingsQuery
    : PagedRequest, IRequest<PagedResult<BuildingDto>>
{

    public Guid? SiteId { get; set; }

    public string? Search { get; set; }
}
