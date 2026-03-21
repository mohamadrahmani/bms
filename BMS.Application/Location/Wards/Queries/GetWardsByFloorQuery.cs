using BMS.Application.Location.Wards.Dtos;
using MediatR;

namespace BMS.Application.Location.Wards.Queries;

public class GetWardsByFloorQuery : IRequest<List<WardDto>>
{
    public Guid FloorId { get; set; }
}
