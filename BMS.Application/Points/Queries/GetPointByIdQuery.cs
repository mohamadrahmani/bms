using BMS.Application.Points.Dtos;
using MediatR;

namespace BMS.Application.Points.Queries;

public class GetPointByIdQuery : IRequest<PointDto?>
{
    public Guid Id { get; set; }

    public GetPointByIdQuery(Guid id)
    {
        Id = id;
    }
}
