using MediatR;
using BMS.Application.Location.Buildings.Dtos;
using BMS.Application.Common.Pagination;

namespace BMS.Application.Location.Buildings.Queries;

public sealed class GetAllBuildingsQuery
    : PagedRequest, IRequest<PagedResult<BuildingDto>>
{
}
