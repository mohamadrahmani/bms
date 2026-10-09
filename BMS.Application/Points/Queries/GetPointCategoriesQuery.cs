using BMS.Application.Common.Pagination;
using BMS.Application.Points.Dtos;
using MediatR;

namespace BMS.Application.Points.Queries;

public class GetPointCategoriesQuery : PagedRequest, IRequest<PagedResult<PointCategoryDto>>
{
    public Guid? DeviceId { get; set; }
}
