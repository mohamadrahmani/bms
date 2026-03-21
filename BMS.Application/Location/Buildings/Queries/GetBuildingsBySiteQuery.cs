using MediatR;
using BMS.Application.Location.Buildings.Dtos;

namespace BMS.Application.Location.Buildings.Queries;

public sealed class GetBuildingsBySiteQuery : IRequest<List<BuildingDto>>
{
    public Guid SiteId { get; set; }
}
