using MediatR;
using BMS.Application.Location.Sites.Dtos;
using BMS.Application.Common.Pagination;
using BMS.Application.Controllers.Dtos;

namespace BMS.Application.Location.Sites.Queries;

public sealed class GetSitesListQuery: PagedRequest, IRequest<PagedResult<SiteDto>>
{
    public string? Search { get; set; }
}
