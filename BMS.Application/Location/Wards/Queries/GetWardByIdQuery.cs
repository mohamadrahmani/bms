using BMS.Application.Location.Wards.Dtos;
using MediatR;

namespace BMS.Application.Location.Wards.Queries;

public class GetWardByIdQuery : IRequest<WardDto?>
{
    public Guid Id { get; set; }
}
