using BMS.Application.Location.Floors.Dtos;
using MediatR;

namespace BMS.Application.Location.Floors.Queries;

public class GetFloorsByBuildingQuery : IRequest<List<FloorDto>>
{
    public Guid BuildingId { get; set; }
}
