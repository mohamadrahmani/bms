using MediatR;
using BMS.Application.Location.Sites.Queries;
using BMS.Application.Location.Sites.Dtos;
using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Pagination;
using BMS.Application.Controllers.Dtos;

namespace BMS.Application.Location.Sites.Handlers;

public sealed class GetSitesListQueryHandler
    : IRequestHandler<GetSitesListQuery, PagedResult<SiteDto>>
{
    private readonly ISiteRepository _siteRepository;

    public GetSitesListQueryHandler(ISiteRepository siteRepository)
    {
        _siteRepository = siteRepository;
    }

    public async Task<PagedResult<SiteDto>> Handle(
        GetSitesListQuery request,
        CancellationToken cancellationToken)
    {
        var sites = _siteRepository.Sites;
        var query = sites
            .Select(site => new SiteDto
            {
                Id = site.Id,
                Name = site.Name,
                Address = site.Address,
                Description = site.Description
            });


        return await query.ToPagedResultAsync(
        request.PageNumber,
        request.PageSize,
        cancellationToken);

    }
}
