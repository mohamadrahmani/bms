using MediatR;
using BMS.Application.Location.Buildings.Dtos;

namespace BMS.Application.Location.Buildings.Queries;

public sealed class GetAllBuildingsQuery
    : IRequest<List<BuildingDto>>
{
}
