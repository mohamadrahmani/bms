using BMS.Application.Common.Pagination;
using BMS.Application.Points.Dtos;
using MediatR;

namespace BMS.Application.Points.Queries;

public class GetPointsQuery : PagedRequest, IRequest<PagedResult<PointDto>>
{
}
