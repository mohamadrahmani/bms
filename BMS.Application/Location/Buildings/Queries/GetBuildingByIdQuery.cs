using MediatR;
using BMS.Application.Location.Buildings.Dtos;

namespace BMS.Application.Location.Buildings.Queries;

public sealed class GetBuildingByIdQuery : IRequest<BuildingDto?>
{
    public Guid Id { get; set; }
}
