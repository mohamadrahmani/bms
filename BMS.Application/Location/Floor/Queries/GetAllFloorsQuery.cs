using BMS.Application.Common.Pagination;
using BMS.Application.Location.Floors.Dtos;
using MediatR;

namespace BMS.Application.Location.Floors.Queries;

public class GetAllFloorsQuery : PagedRequest, IRequest<PagedResult<FloorDto>>
{
}
