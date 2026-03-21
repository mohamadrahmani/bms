using BMS.Application.Location.Floors.Dtos;
using MediatR;

namespace BMS.Application.Location.Floors.Queries;

public class GetAllFloorsQuery : IRequest<List<FloorDto>>
{
}
