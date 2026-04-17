using BMS.Application.Common.Pagination;
using BMS.Application.Controllers.Dtos;
using BMS.Application.Location.Wards.Dtos;
using MediatR;

namespace BMS.Application.Location.Wards.Queries;

public sealed class GetAllWardsQuery : PagedRequest, IRequest<PagedResult<WardDto>>
{
    public Guid? FloorId { get; set; }

    public string? Type { get; set; }

    public string? Search { get; set; }
}


