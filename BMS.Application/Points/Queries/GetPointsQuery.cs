using BMS.Application.Points.Dtos;
using MediatR;

namespace BMS.Application.Points.Queries;

public class GetPointsQuery : IRequest<List<PointDto>>
{
}
