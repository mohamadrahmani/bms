using BMS.Application.Location.Floors.Dtos;
using MediatR;

namespace BMS.Application.Location.Floors.Queries;

public class GetFloorByIdQuery : IRequest<FloorDto?>
{
    public Guid Id { get; set; }
}
